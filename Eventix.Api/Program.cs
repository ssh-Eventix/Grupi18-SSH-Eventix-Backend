using Eventix.API.Middleware;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Application.Services;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Eventix.Infrastructure.Persistence.Repositories;
using Eventix.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.OpenApi.Models;
using Eventix.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Eventix API",
        Version = "v1"
    });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter: Bearer {your JWT}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", jwtScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, new List<string>() }
    });
});

// -------------------- JWT Config --------------------
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSection = builder.Configuration.GetSection("JwtSettings");

var secretKey = jwtSection.GetValue<string>("SecretKey")
                ?? throw new InvalidOperationException("JWT SecretKey is missing");

var issuer = jwtSection.GetValue<string>("Issuer");
var audience = jwtSection.GetValue<string>("Audience");

builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)),

            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var principal = ctx.Principal;
                if (principal == null) return;

                var isImpersonation = principal.HasClaim(c => c.Type == "isImpersonation" && c.Value == "true");
                if (!isImpersonation) return;

                var sessionClaim = principal.FindFirst("impersonationSessionId")?.Value;
                if (!Guid.TryParse(sessionClaim, out var sessionId))
                {
                    ctx.Fail("Invalid impersonation session claim");
                    return;
                }

                var publicDb = ctx.HttpContext.RequestServices.GetService(typeof(PublicDbContext)) as PublicDbContext;
                if (publicDb == null)
                {
                    ctx.Fail("Unable to validate impersonation session");
                    return;
                }

                var session = await publicDb.TenantImpersonationLogs.FindAsync(new object[] { sessionId }, ctx.HttpContext.RequestAborted);
                if (session == null)
                {
                    ctx.Fail("Impersonation session not found");
                    return;
                }

                if (session.ExpiresAtUtc <= DateTime.UtcNow)
                {
                    ctx.Fail("Impersonation session expired");
                    return;
                }
                
                var revoked = await publicDb.TenantImpersonationEvents
                    .AsNoTracking()
                    .AnyAsync(e => e.SessionId == sessionId && e.EventType == Eventix.Domain.Entities.ImpersonationEventType.Revoked, ctx.HttpContext.RequestAborted);

                if (revoked)
                {
                    ctx.Fail("Impersonation session revoked");
                    return;
                }
            }
        };
    });

builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddDbContext<PublicDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<TenantDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
});
builder.Services.AddScoped<ITenantSchemaProvisioner, TenantSchemaProvisioner>();
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IVenueSectionRepository, VenueSectionRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ISpeakerService, SpeakerService>(); 
builder.Services.AddScoped<ISpeakerRepository, SpeakerRepository>();
builder.Services.AddScoped<IEventSectionRepository, EventSectionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IDiscountCouponRepository, DiscountCouponRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IDiscountCouponService, DiscountCouponService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingItemRepository, BookingItemRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
builder.Services.AddScoped<IEventSessionRepository, EventSessionRepository>();
builder.Services.AddScoped<IEventSessionService, EventSessionService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ITicketTypeService, TicketTypeService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IImpersonationService, ImpersonationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClient", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Authorization policies
builder.Services.AddScoped<IAuthorizationHandler, TenantAdminHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminImpersonationHandler>();
// Permission handler & role->permission service
builder.Services.AddSingleton<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TenantAdminOrSuperAdmin", policy =>
    {
        policy.Requirements.Add(new TenantAdminRequirement());
    });

    // Only platform SuperAdmins may start/stop impersonation sessions
    options.AddPolicy("SuperAdminImpersonationOnly", policy =>
    {
        policy.Requirements.Add(new SuperAdminImpersonationRequirement());
    });

    // Permission-based policies (use naming: "Permission:<Name>")
    options.AddPolicy($"Permission:{Permission.EventsCreate}", p => p.Requirements.Add(new PermissionRequirement(Permission.EventsCreate)));
    options.AddPolicy($"Permission:{Permission.EventsRead}", p => p.Requirements.Add(new PermissionRequirement(Permission.EventsRead)));
    options.AddPolicy($"Permission:{Permission.EventsUpdate}", p => p.Requirements.Add(new PermissionRequirement(Permission.EventsUpdate)));
    options.AddPolicy($"Permission:{Permission.EventsDelete}", p => p.Requirements.Add(new PermissionRequirement(Permission.EventsDelete)));

    options.AddPolicy($"Permission:{Permission.TicketsCreate}", p => p.Requirements.Add(new PermissionRequirement(Permission.TicketsCreate)));
    options.AddPolicy($"Permission:{Permission.TicketsRead}", p => p.Requirements.Add(new PermissionRequirement(Permission.TicketsRead)));
    options.AddPolicy($"Permission:{Permission.TicketsUpdate}", p => p.Requirements.Add(new PermissionRequirement(Permission.TicketsUpdate)));
    options.AddPolicy($"Permission:{Permission.TicketsDelete}", p => p.Requirements.Add(new PermissionRequirement(Permission.TicketsDelete)));
    options.AddPolicy($"Permission:{Permission.TicketsPurchase}", p => p.Requirements.Add(new PermissionRequirement(Permission.TicketsPurchase)));

    options.AddPolicy($"Permission:{Permission.VenuesCreate}", p => p.Requirements.Add(new PermissionRequirement(Permission.VenuesCreate)));
    options.AddPolicy($"Permission:{Permission.VenuesRead}", p => p.Requirements.Add(new PermissionRequirement(Permission.VenuesRead)));
    options.AddPolicy($"Permission:{Permission.VenuesUpdate}", p => p.Requirements.Add(new PermissionRequirement(Permission.VenuesUpdate)));
    options.AddPolicy($"Permission:{Permission.VenuesDelete}", p => p.Requirements.Add(new PermissionRequirement(Permission.VenuesDelete)));

    options.AddPolicy($"Permission:{Permission.UsersCreate}", p => p.Requirements.Add(new PermissionRequirement(Permission.UsersCreate)));
    options.AddPolicy($"Permission:{Permission.UsersRead}", p => p.Requirements.Add(new PermissionRequirement(Permission.UsersRead)));
    options.AddPolicy($"Permission:{Permission.UsersUpdate}", p => p.Requirements.Add(new PermissionRequirement(Permission.UsersUpdate)));
    options.AddPolicy($"Permission:{Permission.UsersDelete}", p => p.Requirements.Add(new PermissionRequirement(Permission.UsersDelete)));
    options.AddPolicy($"Permission:{Permission.UsersAssignRoles}", p => p.Requirements.Add(new PermissionRequirement(Permission.UsersAssignRoles)));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

// tenant middleware must run before authentication/authorization so TenantContext is set from header/schema
app.UseMiddleware<TenantMiddleware>();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
