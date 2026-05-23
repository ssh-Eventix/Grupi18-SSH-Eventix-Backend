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

builder.Services.AddDbContext<TenantDbContext>((sp, options) =>
{
    var tenantContext = sp.GetRequiredService<ITenantContext>();
    var schema = tenantContext.SchemaName ?? "public";

    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.MigrationsAssembly("Eventix.Infrastructure");
        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema);
    });

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
builder.Services.AddScoped<ITenantRoleSeeder, TenantRoleSeeder>();
builder.Services.AddScoped<PublicSuperAdminSeeder>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
builder.Services.AddScoped<IEventCategoryService, EventCategoryService>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IVenueSectionRepository, VenueSectionRepository>();
builder.Services.AddScoped<IVenueSectionService, VenueSectionService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ISpeakerService, SpeakerService>(); 
builder.Services.AddScoped<ISpeakerRepository, SpeakerRepository>();
builder.Services.AddScoped<IEventSectionRepository, EventSectionRepository>();
builder.Services.AddScoped<IEventSectionService, EventSectionService>();
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
builder.Services.AddScoped<ITenantEmailDomainService, TenantEmailDomainService>();
builder.Services.AddScoped<ITenantAdminService, TenantAdminService>();
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

    options.AddPolicy("Permission:ViewTenants", policy =>
     policy.Requirements.Add(new PermissionRequirement(Permission.ViewTenants)));

    options.AddPolicy("Permission:CreateTenants", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateTenants)));

    options.AddPolicy("Permission:UpdateTenants", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateTenants)));

    options.AddPolicy("Permission:DeleteTenants", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteTenants)));

    options.AddPolicy("Permission:ImpersonateTenant", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ImpersonateTenant)));

    options.AddPolicy("Permission:ManageUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageUsers)));

    options.AddPolicy("Permission:ViewUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewUsers)));

    options.AddPolicy("Permission:CreateUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateUsers)));

    options.AddPolicy("Permission:UpdateUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateUsers)));

    options.AddPolicy("Permission:DeleteUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteUsers)));

    options.AddPolicy("Permission:ManageRoles", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageRoles)));

    options.AddPolicy("Permission:AssignRoles", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.AssignRoles)));

    options.AddPolicy("Permission:ViewRoles", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewRoles)));

    options.AddPolicy("Permission:SearchEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.SearchEvents)));

    options.AddPolicy("Permission:ViewEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewEvents)));

    options.AddPolicy("Permission:CreateEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateEvents)));

    options.AddPolicy("Permission:UpdateEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateEvents)));

    options.AddPolicy("Permission:DeleteEvents", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteEvents)));

    options.AddPolicy("Permission:ManageVenues", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageVenues)));

    options.AddPolicy("Permission:ViewVenues", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewVenues)));

    options.AddPolicy("Permission:CreateVenues", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateVenues)));

    options.AddPolicy("Permission:UpdateVenues", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateVenues)));

    options.AddPolicy("Permission:DeleteVenues", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteVenues)));

    options.AddPolicy("Permission:ManageVenueSections", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageVenueSections)));

    options.AddPolicy("Permission:ManageEventSections", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageEventSections)));

    options.AddPolicy("Permission:ViewEventSections", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewEventSections)));

    options.AddPolicy("Permission:ManageTicketTypes", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageTicketTypes)));

    options.AddPolicy("Permission:CreateTicketTypes", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateTicketTypes)));

    options.AddPolicy("Permission:UpdateTicketTypes", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateTicketTypes)));

    options.AddPolicy("Permission:DeleteTicketTypes", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteTicketTypes)));

    options.AddPolicy("Permission:ManageBookings", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageBookings)));

    options.AddPolicy("Permission:ViewBookings", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewBookings)));

    options.AddPolicy("Permission:CreateBookings", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateBookings)));

    options.AddPolicy("Permission:CancelBookings", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CancelBookings)));

    options.AddPolicy("Permission:RefundBookings", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.RefundBookings)));

    options.AddPolicy("Permission:ViewTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewTickets)));

    options.AddPolicy("Permission:BuyTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.BuyTickets)));

    options.AddPolicy("Permission:ScanTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ScanTickets)));

    options.AddPolicy("Permission:CheckInTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CheckInTickets)));

    options.AddPolicy("Permission:ValidateTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ValidateTickets)));

    options.AddPolicy("Permission:CancelTickets", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CancelTickets)));

    options.AddPolicy("Permission:ManagePayments", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManagePayments)));

    options.AddPolicy("Permission:ViewPayments", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewPayments)));

    options.AddPolicy("Permission:RefundPayments", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.RefundPayments)));

    options.AddPolicy("Permission:ViewReviews", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewReviews)));

    options.AddPolicy("Permission:DeleteReviews", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteReviews)));

    options.AddPolicy("Permission:CreateReviews", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateReviews)));

    options.AddPolicy("Permission:UpdateReviews", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateReviews)));

    options.AddPolicy("Permission:ViewReports", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewReports)));

    options.AddPolicy("Permission:ViewDashboard", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewDashboard)));

    options.AddPolicy("Permission:ExportReports", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ExportReports)));

    options.AddPolicy("Permission:ManageNotifications", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageNotifications)));

    options.AddPolicy("Permission:ViewNotifications", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewNotifications)));

    options.AddPolicy("Permission:UseAI", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UseAI)));

    options.AddPolicy("Permission:ViewAIRequestLogs", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewAIRequestLogs)));

    options.AddPolicy("Permission:ViewAuditLogs", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewAuditLogs)));

    options.AddPolicy("Permission:ViewArchiveRecords", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewArchiveRecords)));

    options.AddPolicy("Permission:ArchiveRecords", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ArchiveRecords)));

    options.AddPolicy("Permission:ViewEventCategories", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewEventCategories)));

    options.AddPolicy("Permission:CreateEventCategories", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateEventCategories)));

    options.AddPolicy("Permission:UpdateEventCategories", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateEventCategories)));

    options.AddPolicy("Permission:DeleteEventCategories", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteEventCategories)));

    options.AddPolicy("Permission:ManageEventSessions", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageEventSessions)));

    options.AddPolicy("Permission:CreateSpeakers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateSpeakers)));

    options.AddPolicy("Permission:UpdateSpeakers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateSpeakers)));

    options.AddPolicy("Permission:DeleteSpeakers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteSpeakers)));

    options.AddPolicy("Permission:ManageDiscountCoupons", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageDiscountCoupons)));

    options.AddPolicy("Permission:ViewDiscountCoupons", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewDiscountCoupons)));

    options.AddPolicy("Permission:CreateDiscountCoupons", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.CreateDiscountCoupons)));

    options.AddPolicy("Permission:UpdateDiscountCoupons", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.UpdateDiscountCoupons)));

    options.AddPolicy("Permission:DeleteDiscountCoupons", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.DeleteDiscountCoupons)));

    options.AddPolicy("Permission:ViewCheckIns", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewCheckIns)));

    options.AddPolicy("Permission:ManageCheckIns", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageCheckIns)));

    options.AddPolicy("Permission:ManagePaymentMethods", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManagePaymentMethods)));

    options.AddPolicy("Permission:ViewPaymentMethods", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewPaymentMethods)));

    options.AddPolicy("Permission:ManageTenantEmailDomains", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManageTenantEmailDomains)));

    options.AddPolicy("Permission:ViewTenantEmailDomains", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewTenantEmailDomains)));

    options.AddPolicy("Permission:ManagePublicUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManagePublicUsers)));

    options.AddPolicy("Permission:ViewPublicUsers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewPublicUsers)));

    options.AddPolicy("Permission:ManagePublicRoles", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ManagePublicRoles)));

    options.AddPolicy("Permission:ViewPublicRoles", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewPublicRoles)));

    options.AddPolicy("Permission:ViewRefreshTokens", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewRefreshTokens)));

    options.AddPolicy("Permission:RevokeRefreshTokens", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.RevokeRefreshTokens)));

    options.AddPolicy("Permission:ViewEventSessions", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewEventSessions)));

    options.AddPolicy("Permission:ViewSpeakers", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewSpeakers)));

    options.AddPolicy("Permission:ViewTicketTypes", policy =>
        policy.Requirements.Add(new PermissionRequirement(Permission.ViewTicketTypes)));

    options.AddPolicy("Permission:ViewVenueSections", policy =>
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

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<PublicSuperAdminSeeder>();
    await seeder.SeedAsync();
}

app.UseHangfireDashboard("/hangfire");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

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

public partial class Program { }

