using System.Security.Claims;
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
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "role"
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = ImpersonationTokenValidation.ValidateAsync
        };
    });

builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddDbContext<PublicDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<TenantDbContext>((_, options) =>
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

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactClient");

//app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

// tenant middleware must run before authentication/authorization so TenantContext is set from header/schema
app.UseMiddleware<TenantMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        message = "Backend is working",
        time = DateTime.UtcNow
    });
});

app.MapControllers();

app.Run();

public static class ImpersonationAuthConstants
{
    public const string IsImpersonationClaim = "isImpersonation";
    public const string ImpersonationSessionIdClaim = "impersonationSessionId";
}

public static class ImpersonationTokenValidation
{
    public static async Task ValidateAsync(TokenValidatedContext context)
    {
        var principal = context.Principal;
        if (principal == null)
            return;

        var isImpersonation = principal.HasClaim(c =>
            c.Type == ImpersonationAuthConstants.IsImpersonationClaim &&
            c.Value == "true");
        if (!isImpersonation)
            return;

        var sessionClaim = principal.FindFirst(ImpersonationAuthConstants.ImpersonationSessionIdClaim)?.Value;
        if (!Guid.TryParse(sessionClaim, out var sessionId))
        {
            context.Fail("Invalid impersonation session claim");
            return;
        }

        var publicDb = context.HttpContext.RequestServices.GetRequiredService<PublicDbContext>();
        var tenantContext = context.HttpContext.RequestServices.GetRequiredService<ITenantContext>();

        var session = await publicDb.TenantImpersonationLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == sessionId, context.HttpContext.RequestAborted);

        if (session == null)
        {
            context.Fail("Impersonation session not found");
            return;
        }

        if (session.TenantId != tenantContext.TenantId)
        {
            context.Fail("Tenant mismatch");
            return;
        }

        if (!session.IsActive || session.RevokedAtUtc.HasValue)
        {
            context.Fail("Impersonation session revoked");
            return;
        }

        if (session.ExpiresAtUtc <= DateTime.UtcNow)
        {
            context.Fail("Impersonation session expired");
            return;
        }

        var subjectClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(subjectClaim, out var subjectUserId))
        {
            context.Fail("Invalid subject claim");
            return;
        }

        if (session.TargetTenantUserId != subjectUserId)
            context.Fail("Impersonation subject mismatch");
    }
}
