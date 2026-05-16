using Eventix.API.Middleware;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Application.Services;
using Eventix.Infrastructure.Auth;
using Eventix.Infrastructure.BackgroundJobs;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Eventix.Infrastructure.Persistence.Repositories;
using Eventix.Infrastructure.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Eventix.API.Authorization;
using Eventix.Domain.Enums;
using DotNetEnv;


var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Configuration["JwtSettings:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
builder.Configuration["JwtSettings:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER");
builder.Configuration["JwtSettings:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
builder.Configuration["JwtSettings:ExpirationMinutes"] = Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES");

var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

if (string.IsNullOrWhiteSpace(dbHost) ||
    string.IsNullOrWhiteSpace(dbPort) ||
    string.IsNullOrWhiteSpace(dbName) ||
    string.IsNullOrWhiteSpace(dbUser) ||
    string.IsNullOrWhiteSpace(dbPassword))
{
    throw new InvalidOperationException(
        "Database environment variables are missing.");
}



var connectionString =
    $"Host={dbHost};" +
    $"Port={dbPort};" +
    $"Database={dbName};" +
    $"Username={dbUser};" +
    $"Password={dbPassword}";

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

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

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
    options.UseNpgsql(connectionString));

builder.Services.AddDbContext<TenantDbContext>((_, options) =>
{
    options.UseNpgsql(connectionString);
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
builder.Services.AddScoped<ICheckInRepository, CheckInRepository>();
builder.Services.AddScoped<ICheckInService, CheckInService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPublicUserRepository, PublicUserRepository>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ImpersonationHandler>();
builder.Services.AddScoped<ITenantEmailDomainRepository, TenantEmailDomainRepository>();
builder.Services.AddScoped<BookingCleanupJob>();
builder.Services.AddScoped<NotificationReminderJob>();
builder.Services.AddScoped<TicketExpirationJob>();
builder.Services.AddScoped<ReviewReminderJob>();
builder.Services.AddScoped<PaymentRetryJob>();
builder.Services.AddScoped<EventStatusUpdateJob>();
builder.Services.AddScoped<CouponExpirationJob>();
builder.Services.AddScoped<CheckInAnalyticsJob>();
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

    options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireRole("SuperAdmin"));

    options.AddPolicy("TenantAdminOnly", policy =>
        policy.RequireRole("Admin", "SuperAdmin"));

    options.AddPolicy("CanImpersonateTenant", policy =>
        policy.Requirements.Add(new SuperAdminImpersonationRequirement()));

    options.AddPolicy("ManageEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageEvents)));

    options.AddPolicy("ManageUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageUsers)));

    options.AddPolicy("ScanTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ScanTickets)));

    options.AddPolicy("BuyTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.BuyTickets)));
});

// hangfire
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(connectionString);
    });
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHangfireDashboard("/hangfire");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("ReactClient");

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
})
.AllowAnonymous();

app.MapControllers();

JobScheduler.RegisterJobs();

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

        if (session.TargetTenantId != tenantContext.TenantId)
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

    }
}
