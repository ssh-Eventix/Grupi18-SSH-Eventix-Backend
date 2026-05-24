using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class InitialTenantSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "EventCategory",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Speaker",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Bio = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speakers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venue",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AddressLine1 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TotalCapacity = table.Column<int>(type: "integer", nullable: false),
                    IsIndoor = table.Column<bool>(type: "boolean", nullable: false),
                    IsAccessible = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                    table.CheckConstraint("CK_Venue_TotalCapacity", "\"TotalCapacity\" >= 0");
                });

            migrationBuilder.CreateTable(
                name: "AIRequestLog",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Prompt = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    ResponseSummary = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RequestType = table.Column<int>(type: "integer", nullable: false),
                    TokensUsed = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRequestLogs", x => x.Id);
                    table.CheckConstraint("CK_AIRequestLog_TokensUsed", "\"TokensUsed\" >= 0");
                    table.ForeignKey(
                        name: "FK_AIRequestLogs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    OldValues = table.Column<string>(type: "jsonb", nullable: true),
                    NewValues = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Slug = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    OrganizerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false),
                    BannerImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MaxTicketsPerOrder = table.Column<int>(type: "integer", nullable: false),
                    MinTicketsPerOrder = table.Column<int>(type: "integer", nullable: false),
                    IsFree = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.CheckConstraint("CK_Event_DateRange", "\"EndUtc\" > \"StartUtc\"");
                    table.CheckConstraint("CK_Event_MaxTicketsPerOrder", "\"MaxTicketsPerOrder\" >= \"MinTicketsPerOrder\"");
                    table.CheckConstraint("CK_Event_MinTicketsPerOrder", "\"MinTicketsPerOrder\" > 0");
                    table.ForeignKey(
                        name: "FK_Events_EventCategories_EventCategoryId",
                        column: x => x.EventCategoryId,
                        principalSchema: "public",
                        principalTable: "EventCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "public",
                        principalTable: "Venue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VenueSection",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    SeatType = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultBasePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueSections", x => x.Id);
                    table.CheckConstraint("CK_VenueSection_Capacity", "\"Capacity\" >= 0");
                    table.CheckConstraint("CK_VenueSection_DefaultBasePrice", "\"DefaultBasePrice\" IS NULL OR \"DefaultBasePrice\" >= 0");
                    table.ForeignKey(
                        name: "FK_VenueSections_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "public",
                        principalTable: "Venue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.CheckConstraint("CK_Booking_TotalAmount", "\"TotalAmount\" >= 0");
                    table.ForeignKey(
                        name: "FK_Bookings_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCoupon",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsageLimit = table.Column<int>(type: "integer", nullable: true),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCoupons", x => x.Id);
                    table.CheckConstraint("CK_DiscountCoupon_DiscountValue", "\"DiscountValue\" > 0");
                    table.CheckConstraint("CK_DiscountCoupon_UsageCount", "\"UsageCount\" >= 0");
                    table.CheckConstraint("CK_DiscountCoupon_UsageLimit", "\"UsageLimit\" IS NULL OR \"UsageLimit\" > 0");
                    table.CheckConstraint("CK_DiscountCoupon_ValidRange", "\"ValidTo\" > \"ValidFrom\"");
                    table.ForeignKey(
                        name: "FK_DiscountCoupons_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventSession",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpeakerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSessions", x => x.Id);
                    table.CheckConstraint("CK_EventSession_TimeRange", "\"EndTime\" > \"StartTime\"");
                    table.ForeignKey(
                        name: "FK_EventSessions_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSessions_Speakers_SpeakerId",
                        column: x => x.SpeakerId,
                        principalSchema: "public",
                        principalTable: "Speaker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.CheckConstraint("CK_Review_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5");
                    table.ForeignKey(
                        name: "FK_Reviews_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventSection",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueSectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SalesStartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SalesEndUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSections", x => x.Id);
                    table.CheckConstraint("CK_EventSection_Capacity", "\"Capacity\" >= 0");
                    table.CheckConstraint("CK_EventSection_SalesRange", "\"SalesStartUtc\" IS NULL OR \"SalesEndUtc\" IS NULL OR \"SalesEndUtc\" > \"SalesStartUtc\"");
                    table.ForeignKey(
                        name: "FK_EventSections_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSections_VenueSections_VenueSectionId",
                        column: x => x.VenueSectionId,
                        principalSchema: "public",
                        principalTable: "VenueSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.CheckConstraint("CK_Payment_Amount", "\"Amount\" > 0");
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "public",
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "public",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    QuantityAvailable = table.Column<int>(type: "integer", nullable: false),
                    SoldQuantity = table.Column<int>(type: "integer", nullable: false),
                    SaleStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SaleEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventSectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                    table.CheckConstraint("CK_TicketType_Price", "\"Price\" >= 0");
                    table.CheckConstraint("CK_TicketType_QuantityAvailable", "\"QuantityAvailable\" >= 0");
                    table.CheckConstraint("CK_TicketType_SaleRange", "\"SaleStartDate\" IS NULL OR \"SaleEndDate\" IS NULL OR \"SaleEndDate\" > \"SaleStartDate\"");
                    table.CheckConstraint("CK_TicketType_SoldQuantity", "\"SoldQuantity\" >= 0");
                    table.CheckConstraint("CK_TicketType_SoldQuantity_Limit", "\"SoldQuantity\" <= \"QuantityAvailable\"");
                    table.ForeignKey(
                        name: "FK_TicketTypes_EventSections_EventSectionId",
                        column: x => x.EventSectionId,
                        principalSchema: "public",
                        principalTable: "EventSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketTypes_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingItem",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EventSectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingItems", x => x.Id);
                    table.CheckConstraint("CK_BookingItem_Quantity", "\"Quantity\" > 0");
                    table.CheckConstraint("CK_BookingItem_UnitPrice", "\"UnitPrice\" >= 0");
                    table.ForeignKey(
                        name: "FK_BookingItems_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "public",
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingItems_EventSections_EventSectionId",
                        column: x => x.EventSectionId,
                        principalSchema: "public",
                        principalTable: "EventSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingItems_TicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalSchema: "public",
                        principalTable: "TicketType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    QRCode = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_BookingItems_BookingItemId",
                        column: x => x.BookingItemId,
                        principalSchema: "public",
                        principalTable: "BookingItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckIn",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckedInByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckIns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckIns_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "public",
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckIns_Users_CheckedInByUserId",
                        column: x => x.CheckedInByUserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIRequestLogs_TenantId_UserId_CreatedAt",
                schema: "public",
                table: "AIRequestLog",
                columns: new[] { "TenantId", "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AIRequestLogs_UserId",
                schema: "public",
                table: "AIRequestLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_EntityName_EntityId",
                schema: "public",
                table: "AuditLog",
                columns: new[] { "TenantId", "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_UserId_CreatedAt",
                schema: "public",
                table: "AuditLog",
                columns: new[] { "TenantId", "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                schema: "public",
                table: "AuditLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_BookingId",
                schema: "public",
                table: "BookingItem",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_EventSectionId",
                schema: "public",
                table: "BookingItem",
                column: "EventSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_TicketTypeId",
                schema: "public",
                table: "BookingItem",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EventId",
                schema: "public",
                table: "Booking",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TenantId_ReferenceNumber",
                schema: "public",
                table: "Booking",
                columns: new[] { "TenantId", "ReferenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                schema: "public",
                table: "Booking",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_CheckedInByUserId",
                schema: "public",
                table: "CheckIn",
                column: "CheckedInByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_TenantId_TicketId",
                schema: "public",
                table: "CheckIn",
                columns: new[] { "TenantId", "TicketId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_TicketId",
                schema: "public",
                table: "CheckIn",
                column: "TicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCoupons_EventId",
                schema: "public",
                table: "DiscountCoupon",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCoupons_TenantId_EventId_Code",
                schema: "public",
                table: "DiscountCoupon",
                columns: new[] { "TenantId", "EventId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_TenantId_Name",
                schema: "public",
                table: "EventCategory",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventCategoryId",
                schema: "public",
                table: "Event",
                column: "EventCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_TenantId_Slug",
                schema: "public",
                table: "Event",
                columns: new[] { "TenantId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_VenueId",
                schema: "public",
                table: "Event",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSections_EventId",
                schema: "public",
                table: "EventSection",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSections_TenantId_EventId_Code",
                schema: "public",
                table: "EventSection",
                columns: new[] { "TenantId", "EventId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventSections_TenantId_EventId_VenueSectionId",
                schema: "public",
                table: "EventSection",
                columns: new[] { "TenantId", "EventId", "VenueSectionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventSections_VenueSectionId",
                schema: "public",
                table: "EventSection",
                column: "VenueSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_EventId",
                schema: "public",
                table: "EventSession",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_SpeakerId",
                schema: "public",
                table: "EventSession",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EventId",
                schema: "public",
                table: "Notification",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TenantId_UserId_IsRead",
                schema: "public",
                table: "Notification",
                columns: new[] { "TenantId", "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "public",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_TenantId_Name",
                schema: "public",
                table: "PaymentMethod",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                schema: "public",
                table: "Payment",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodId",
                schema: "public",
                table: "Payment",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TenantId_TransactionId",
                schema: "public",
                table: "Payment",
                columns: new[] { "TenantId", "TransactionId" },
                unique: true,
                filter: "\"TransactionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EventId",
                schema: "public",
                table: "Review",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TenantId_EventId_UserId",
                schema: "public",
                table: "Review",
                columns: new[] { "TenantId", "EventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                schema: "public",
                table: "Review",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId_Name",
                schema: "public",
                table: "Role",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_TenantId_Email",
                schema: "public",
                table: "Speaker",
                columns: new[] { "TenantId", "Email" },
                unique: true,
                filter: "\"Email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_BookingItemId",
                schema: "public",
                table: "Ticket",
                column: "BookingItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TenantId_QRCode",
                schema: "public",
                table: "Ticket",
                columns: new[] { "TenantId", "QRCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TenantId_TicketCode",
                schema: "public",
                table: "Ticket",
                columns: new[] { "TenantId", "TicketCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_EventId",
                schema: "public",
                table: "TicketType",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_EventSectionId",
                schema: "public",
                table: "TicketType",
                column: "EventSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_TenantId_EventId_Name",
                schema: "public",
                table: "TicketType",
                columns: new[] { "TenantId", "EventId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "public",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TenantId_UserId_RoleId",
                schema: "public",
                table: "UserRole",
                columns: new[] { "TenantId", "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                schema: "public",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Email",
                schema: "public",
                table: "User",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Venues_TenantId_Code",
                schema: "public",
                table: "Venue",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VenueSections_TenantId_VenueId_Code",
                schema: "public",
                table: "VenueSection",
                columns: new[] { "TenantId", "VenueId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VenueSections_VenueId",
                schema: "public",
                table: "VenueSection",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIRequestLog",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AuditLog",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CheckIn",
                schema: "public");

            migrationBuilder.DropTable(
                name: "DiscountCoupon",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventSession",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Review",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Ticket",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Speaker",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PaymentMethod",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BookingItem",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Booking",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TicketType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "User",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventSection",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Event",
                schema: "public");

            migrationBuilder.DropTable(
                name: "VenueSection",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventCategory",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Venue",
                schema: "public");
        }
    }
}

