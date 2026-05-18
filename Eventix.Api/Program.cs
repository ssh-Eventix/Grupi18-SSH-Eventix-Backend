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

    var tenantScheme = new OpenApiSecurityScheme
    {
        Name = "X-Tenant-Slug",
        Description = "Enter tenant slug, for example: eventix-test",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Tenant"
    };

    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityDefinition("Tenant", tenantScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Tenant"
                }
            },
            new List<string>()
        }
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

// REDIS CACHE
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "Eventix_";
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

    options.AddPolicy("ViewTenants", policy =>
     policy.Requirements.Add(new PermissionRequirement(Permission.ViewTenants)));

    options.AddPolicy("CreateTenants", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateTenants)));

    options.AddPolicy("UpdateTenants", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateTenants)));

    options.AddPolicy("DeleteTenants", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteTenants)));

    options.AddPolicy("ImpersonateTenant", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ImpersonateTenant)));

    options.AddPolicy("ManageUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageUsers)));

    options.AddPolicy("ViewUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewUsers)));

    options.AddPolicy("CreateUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateUsers)));

    options.AddPolicy("UpdateUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateUsers)));

    options.AddPolicy("DeleteUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteUsers)));

    options.AddPolicy("ManageRoles", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageRoles)));

    options.AddPolicy("AssignRoles", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.AssignRoles)));

    options.AddPolicy("ViewRoles", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewRoles)));

    options.AddPolicy("SearchEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.SearchEvents)));

    options.AddPolicy("ViewEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewEvents)));

    options.AddPolicy("CreateEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateEvents)));

    options.AddPolicy("UpdateEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateEvents)));

    options.AddPolicy("DeleteEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteEvents)));

    options.AddPolicy("ManageVenues", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageVenues)));

    options.AddPolicy("ViewVenues", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewVenues)));

    options.AddPolicy("CreateVenues", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateVenues)));

    options.AddPolicy("UpdateVenues", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateVenues)));

    options.AddPolicy("DeleteVenues", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteVenues)));

    options.AddPolicy("ManageVenueSections", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageVenueSections)));

    options.AddPolicy("ManageEventSections", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageEventSections)));

    options.AddPolicy("ViewEventSections", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewEventSections)));

    options.AddPolicy("ManageTicketTypes", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageTicketTypes)));

    options.AddPolicy("CreateTicketTypes", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateTicketTypes)));

    options.AddPolicy("UpdateTicketTypes", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateTicketTypes)));

    options.AddPolicy("DeleteTicketTypes", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteTicketTypes)));

    options.AddPolicy("ManageBookings", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageBookings)));

    options.AddPolicy("ViewBookings", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewBookings)));

    options.AddPolicy("CreateBookings", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateBookings)));

    options.AddPolicy("CancelBookings", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CancelBookings)));

    options.AddPolicy("RefundBookings", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.RefundBookings)));

    options.AddPolicy("ViewTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewTickets)));

    options.AddPolicy("BuyTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.BuyTickets)));

    options.AddPolicy("ScanTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ScanTickets)));

    options.AddPolicy("CheckInTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CheckInTickets)));

    options.AddPolicy("ValidateTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ValidateTickets)));

    options.AddPolicy("CancelTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CancelTickets)));

    options.AddPolicy("ManagePayments", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManagePayments)));

    options.AddPolicy("ViewPayments", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewPayments)));

    options.AddPolicy("RefundPayments", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.RefundPayments)));

    options.AddPolicy("ViewReviews", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewReviews)));

    options.AddPolicy("DeleteReviews", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteReviews)));

    options.AddPolicy("CreateReviews", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateReviews)));

    options.AddPolicy("UpdateReviews", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateReviews)));

    options.AddPolicy("ViewReports", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewReports)));

    options.AddPolicy("ViewDashboard", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewDashboard)));

    options.AddPolicy("ExportReports", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ExportReports)));

    options.AddPolicy("ManageNotifications", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageNotifications)));

    options.AddPolicy("ViewNotifications", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewNotifications)));

    options.AddPolicy("UseAI", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UseAI)));

    options.AddPolicy("ViewAIRequestLogs", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewAIRequestLogs)));

    options.AddPolicy("ViewAuditLogs", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewAuditLogs)));

    options.AddPolicy("ViewArchiveRecords", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewArchiveRecords)));

    options.AddPolicy("ArchiveRecords", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ArchiveRecords)));

    options.AddPolicy("ViewEventCategories", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewEventCategories)));

    options.AddPolicy("CreateEventCategories", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateEventCategories)));

    options.AddPolicy("UpdateEventCategories", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateEventCategories)));

    options.AddPolicy("DeleteEventCategories", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteEventCategories)));

    options.AddPolicy("ManageEventSessions", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageEventSessions)));

    options.AddPolicy("CreateSpeakers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateSpeakers)));

    options.AddPolicy("UpdateSpeakers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateSpeakers)));

    options.AddPolicy("DeleteSpeakers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteSpeakers)));

    options.AddPolicy("ManageDiscountCoupons", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageDiscountCoupons)));

    options.AddPolicy("ViewDiscountCoupons", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewDiscountCoupons)));

    options.AddPolicy("CreateDiscountCoupons", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateDiscountCoupons)));

    options.AddPolicy("UpdateDiscountCoupons", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateDiscountCoupons)));

    options.AddPolicy("DeleteDiscountCoupons", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteDiscountCoupons)));

    options.AddPolicy("ViewCheckIns", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewCheckIns)));

    options.AddPolicy("ManageCheckIns", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageCheckIns)));

    options.AddPolicy("ManagePaymentMethods", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManagePaymentMethods)));

    options.AddPolicy("ViewPaymentMethods", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewPaymentMethods)));

    options.AddPolicy("ManageTenantEmailDomains", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageTenantEmailDomains)));

    options.AddPolicy("ViewTenantEmailDomains", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewTenantEmailDomains)));

    options.AddPolicy("ManagePublicUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManagePublicUsers)));

    options.AddPolicy("ViewPublicUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewPublicUsers)));

    options.AddPolicy("ManagePublicRoles", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManagePublicRoles)));

    options.AddPolicy("ViewPublicRoles", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewPublicRoles)));

    options.AddPolicy("ViewRefreshTokens", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewRefreshTokens)));

    options.AddPolicy("RevokeRefreshTokens", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.RevokeRefreshTokens)));

    options.AddPolicy("ViewEventSessions", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewEventSessions)));

    options.AddPolicy("ViewSpeakers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewSpeakers)));

    options.AddPolicy("ViewTicketTypes", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewTicketTypes)));

    options.AddPolicy("ViewVenueSections", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewVenueSections)));
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

app.UseHttpsRedirection();

app.UseCors("ReactClient");

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseMiddleware<TenantMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        message = "Backend is working",
        time = DateTime.UtcNow
    });
})
.AllowAnonymous();

JobScheduler.RegisterJobs();

app.Run();

