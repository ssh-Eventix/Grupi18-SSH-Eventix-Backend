using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                schema: "public",
                table: "Tenants");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTrial",
                schema: "public",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxEvents",
                schema: "public",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxUsers",
                schema: "public",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "public",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionEndUtc",
                schema: "public",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionStartUtc",
                schema: "public",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventCategory",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venue",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AddressLine1 = table.Column<string>(type: "text", nullable: false),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalCapacity = table.Column<int>(type: "integer", nullable: false),
                    ContactEmail = table.Column<string>(type: "text", nullable: true),
                    ContactPhone = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    HasSeatingMap = table.Column<bool>(type: "boolean", nullable: false),
                    SeatingMapImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsIndoor = table.Column<bool>(type: "boolean", nullable: false),
                    IsAccessible = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Venue_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "public",
                        principalTable: "Tenants",
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
                    Title = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Subtitle = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    OrganizerName = table.Column<string>(type: "text", nullable: true),
                    OrganizerEmail = table.Column<string>(type: "text", nullable: true),
                    OrganizerPhone = table.Column<string>(type: "text", nullable: true),
                    StartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DoorsOpenUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SalesStartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SalesEndUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false),
                    BannerImageUrl = table.Column<string>(type: "text", nullable: true),
                    ThumbnailImageUrl = table.Column<string>(type: "text", nullable: true),
                    EventCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    MaxTicketsPerOrder = table.Column<int>(type: "integer", nullable: false),
                    MinTicketsPerOrder = table.Column<int>(type: "integer", nullable: false),
                    IsFree = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    AllowWaitlist = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    TermsAndConditions = table.Column<string>(type: "text", nullable: true),
                    RefundPolicy = table.Column<string>(type: "text", nullable: true),
                    MinPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_EventCategory_EventCategoryId",
                        column: x => x.EventCategoryId,
                        principalSchema: "public",
                        principalTable: "EventCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "public",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_Venue_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "public",
                        principalTable: "Venue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VenueSection",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    SeatType = table.Column<int>(type: "integer", nullable: false),
                    RowCount = table.Column<int>(type: "integer", nullable: true),
                    SeatsPerRow = table.Column<int>(type: "integer", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsAccessibleSection = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultBasePrice = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueSection_Venue_VenueId",
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
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Booking_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventSessions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SpeakerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSessions_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSessions_Speakers_SpeakerId",
                        column: x => x.SpeakerId,
                        principalSchema: "public",
                        principalTable: "Speakers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    QuantityAvailable = table.Column<int>(type: "integer", nullable: false),
                    SoldQuantity = table.Column<int>(type: "integer", nullable: false),
                    SaleStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SaleEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketType_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventSection",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueSectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    ReservedSeats = table.Column<int>(type: "integer", nullable: false),
                    SoldSeats = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    SalesEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    SalesStartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SalesEndUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxTicketsPerOrder = table.Column<int>(type: "integer", nullable: false),
                    MinTicketsPerOrder = table.Column<int>(type: "integer", nullable: false),
                    Benefits = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSection_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSection_VenueSection_VenueSectionId",
                        column: x => x.VenueSectionId,
                        principalSchema: "public",
                        principalTable: "VenueSection",
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
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingItem_Booking_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "public",
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingItem_TicketType_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalSchema: "public",
                        principalTable: "TicketType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketCode = table.Column<string>(type: "text", nullable: false),
                    QRCode = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_BookingItem_BookingItemId",
                        column: x => x.BookingItemId,
                        principalSchema: "public",
                        principalTable: "BookingItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckIns",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckedInByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckIns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckIns_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "public",
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckIns_User_CheckedInByUserId",
                        column: x => x.CheckedInByUserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_EventId",
                schema: "public",
                table: "Booking",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_UserId",
                schema: "public",
                table: "Booking",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItem_BookingId",
                schema: "public",
                table: "BookingItem",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItem_TicketTypeId",
                schema: "public",
                table: "BookingItem",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_CheckedInByUserId",
                schema: "public",
                table: "CheckIns",
                column: "CheckedInByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_TicketId",
                schema: "public",
                table: "CheckIns",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventCategoryId",
                schema: "public",
                table: "Event",
                column: "EventCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_TenantId",
                schema: "public",
                table: "Event",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_VenueId",
                schema: "public",
                table: "Event",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSection_EventId",
                schema: "public",
                table: "EventSection",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSection_VenueSectionId",
                schema: "public",
                table: "EventSection",
                column: "VenueSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_EventId",
                schema: "public",
                table: "EventSessions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_SpeakerId",
                schema: "public",
                table: "EventSessions",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EventId",
                schema: "public",
                table: "Notifications",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "public",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EventId",
                schema: "public",
                table: "Reviews",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                schema: "public",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_BookingItemId",
                schema: "public",
                table: "Ticket",
                column: "BookingItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketType_EventId",
                schema: "public",
                table: "TicketType",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Venue_TenantId",
                schema: "public",
                table: "Venue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueSection_VenueId",
                schema: "public",
                table: "VenueSection",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckIns",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventSection",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventSessions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Ticket",
                schema: "public");

            migrationBuilder.DropTable(
                name: "VenueSection",
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
                name: "Event",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EventCategory",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Venue",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Country",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsTrial",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "MaxEvents",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "MaxUsers",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "State",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SubscriptionEndUtc",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SubscriptionStartUtc",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                schema: "public",
                table: "Tenants");

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                schema: "public",
                table: "Tenants",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
