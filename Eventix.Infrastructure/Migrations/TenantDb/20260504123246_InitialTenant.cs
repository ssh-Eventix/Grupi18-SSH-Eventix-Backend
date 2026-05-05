using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class InitialTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_TicketTypes_TicketTypeId",
                schema: "public",
                table: "BookingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Events_EventId",
                schema: "public",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                schema: "public",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventCategories_EventCategoryId",
                schema: "public",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueId",
                schema: "public",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueId1",
                schema: "public",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSections_VenueSections_VenueSectionId",
                schema: "public",
                table: "EventSections");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSessions_Speakers_SpeakerId",
                schema: "public",
                table: "EventSessions");

            migrationBuilder.DropIndex(
                name: "IX_VenueSections_TenantId",
                schema: "public",
                table: "VenueSections");

            migrationBuilder.DropIndex(
                name: "IX_Venues_TenantId",
                schema: "public",
                table: "Venues");

            migrationBuilder.DropIndex(
                name: "IX_TicketTypes_TenantId",
                schema: "public",
                table: "TicketTypes");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TenantId",
                schema: "public",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketCode",
                schema: "public",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_EventSessions_TenantId",
                schema: "public",
                table: "EventSessions");

            migrationBuilder.DropIndex(
                name: "IX_EventSections_TenantId",
                schema: "public",
                table: "EventSections");

            migrationBuilder.DropIndex(
                name: "IX_Events_TenantId",
                schema: "public",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_VenueId1",
                schema: "public",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_EventCategories_TenantId",
                schema: "public",
                table: "EventCategories");

            migrationBuilder.DropIndex(
                name: "IX_EventCategories_TenantId_Name",
                schema: "public",
                table: "EventCategories");

            migrationBuilder.DropIndex(
                name: "IX_CheckIns_TicketId",
                schema: "public",
                table: "CheckIns");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TenantId",
                schema: "public",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_BookingItems_TenantId",
                schema: "public",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "Price",
                schema: "public",
                table: "EventSections");

            migrationBuilder.DropColumn(
                name: "VenueId1",
                schema: "public",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "VenueSections",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "VenueSections",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                schema: "public",
                table: "VenueSections",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultBasePrice",
                schema: "public",
                table: "VenueSections",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "VenueSections",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Venues",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsIndoor",
                schema: "public",
                table: "Venues",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAccessible",
                schema: "public",
                table: "Venues",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "public",
                table: "Venues",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Venues",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "public",
                table: "Venues",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine1",
                schema: "public",
                table: "Venues",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "public",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "UserRoles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "SoldQuantity",
                schema: "public",
                table: "TicketTypes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleStartDate",
                schema: "public",
                table: "TicketTypes",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleEndDate",
                schema: "public",
                table: "TicketTypes",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                schema: "public",
                table: "TicketTypes",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "TicketTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<Guid>(
                name: "EventSectionId",
                schema: "public",
                table: "TicketTypes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TicketCode",
                schema: "public",
                table: "Tickets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "IssuedAt",
                schema: "public",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "UsedAt",
                schema: "public",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "public",
                table: "Speakers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "Speakers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "Speakers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "public",
                table: "Reviews",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "public",
                table: "Reviews",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "Reviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "Reviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                schema: "public",
                table: "Notifications",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "public",
                table: "Notifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "public",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "Notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "Notifications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "public",
                table: "EventSessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "EventSessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "EventSections",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "EventSections",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "EventSections",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "public",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                schema: "public",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizerName",
                schema: "public",
                table: "Events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MinTicketsPerOrder",
                schema: "public",
                table: "Events",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "MaxTicketsPerOrder",
                schema: "public",
                table: "Events",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 10);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublished",
                schema: "public",
                table: "Events",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsFree",
                schema: "public",
                table: "Events",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "Events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                schema: "public",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "BannerImageUrl",
                schema: "public",
                table: "Events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "EventCategories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "EventCategories",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                schema: "public",
                table: "EventCategories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                schema: "public",
                table: "EventCategories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "EventCategories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                schema: "public",
                table: "CheckIns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "public",
                table: "CheckIns",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "CheckIns",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "CheckIns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "CheckIns",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                schema: "public",
                table: "Bookings",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                schema: "public",
                table: "Bookings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                schema: "public",
                table: "BookingItems",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddColumn<Guid>(
                name: "EventSectionId",
                schema: "public",
                table: "BookingItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
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
                name: "Payments",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "public",
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "public",
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Email",
                schema: "public",
                table: "Users",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TenantId_UserId_RoleId",
                schema: "public",
                table: "UserRoles",
                columns: new[] { "TenantId", "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_EventSectionId",
                schema: "public",
                table: "TicketTypes",
                column: "EventSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_QRCode",
                schema: "public",
                table: "Tickets",
                column: "QRCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TenantId_TicketCode",
                schema: "public",
                table: "Tickets",
                columns: new[] { "TenantId", "TicketCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId_Name",
                schema: "public",
                table: "Roles",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TenantId_EventId_UserId",
                schema: "public",
                table: "Reviews",
                columns: new[] { "TenantId", "EventId", "UserId" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Review_Rating",
                schema: "public",
                table: "Reviews",
                sql: "\"Rating\" >= 1 AND \"Rating\" <= 5");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCoupons_TenantId_Code",
                schema: "public",
                table: "DiscountCoupons",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_TenantId_TicketId",
                schema: "public",
                table: "CheckIns",
                columns: new[] { "TenantId", "TicketId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_TicketId",
                schema: "public",
                table: "CheckIns",
                column: "TicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TenantId_ReferenceNumber",
                schema: "public",
                table: "Bookings",
                columns: new[] { "TenantId", "ReferenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_EventSectionId",
                schema: "public",
                table: "BookingItems",
                column: "EventSectionId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Quantity",
                schema: "public",
                table: "BookingItems",
                sql: "\"Quantity\" > 0");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_TenantId_Name",
                schema: "public",
                table: "PaymentMethods",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                schema: "public",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodId",
                schema: "public",
                table: "Payments",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_EventSections_EventSectionId",
                schema: "public",
                table: "BookingItems",
                column: "EventSectionId",
                principalSchema: "public",
                principalTable: "EventSections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_TicketTypes_TicketTypeId",
                schema: "public",
                table: "BookingItems",
                column: "TicketTypeId",
                principalSchema: "public",
                principalTable: "TicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Events_EventId",
                schema: "public",
                table: "Bookings",
                column: "EventId",
                principalSchema: "public",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                schema: "public",
                table: "Bookings",
                column: "UserId",
                principalSchema: "public",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventCategories_EventCategoryId",
                schema: "public",
                table: "Events",
                column: "EventCategoryId",
                principalSchema: "public",
                principalTable: "EventCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venues_VenueId",
                schema: "public",
                table: "Events",
                column: "VenueId",
                principalSchema: "public",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventSections_VenueSections_VenueSectionId",
                schema: "public",
                table: "EventSections",
                column: "VenueSectionId",
                principalSchema: "public",
                principalTable: "VenueSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventSessions_Speakers_SpeakerId",
                schema: "public",
                table: "EventSessions",
                column: "SpeakerId",
                principalSchema: "public",
                principalTable: "Speakers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTypes_EventSections_EventSectionId",
                schema: "public",
                table: "TicketTypes",
                column: "EventSectionId",
                principalSchema: "public",
                principalTable: "EventSections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_EventSections_EventSectionId",
                schema: "public",
                table: "BookingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_TicketTypes_TicketTypeId",
                schema: "public",
                table: "BookingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Events_EventId",
                schema: "public",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                schema: "public",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventCategories_EventCategoryId",
                schema: "public",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueId",
                schema: "public",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSections_VenueSections_VenueSectionId",
                schema: "public",
                table: "EventSections");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSessions_Speakers_SpeakerId",
                schema: "public",
                table: "EventSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTypes_EventSections_EventSectionId",
                schema: "public",
                table: "TicketTypes");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PaymentMethods",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId_Email",
                schema: "public",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_TenantId_UserId_RoleId",
                schema: "public",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_TicketTypes_EventSectionId",
                schema: "public",
                table: "TicketTypes");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_QRCode",
                schema: "public",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TenantId_TicketCode",
                schema: "public",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Roles_TenantId_Name",
                schema: "public",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_TenantId_EventId_UserId",
                schema: "public",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Review_Rating",
                schema: "public",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_DiscountCoupons_TenantId_Code",
                schema: "public",
                table: "DiscountCoupons");

            migrationBuilder.DropIndex(
                name: "IX_CheckIns_TenantId_TicketId",
                schema: "public",
                table: "CheckIns");

            migrationBuilder.DropIndex(
                name: "IX_CheckIns_TicketId",
                schema: "public",
                table: "CheckIns");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TenantId_ReferenceNumber",
                schema: "public",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_BookingItems_EventSectionId",
                schema: "public",
                table: "BookingItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Quantity",
                schema: "public",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "EventSectionId",
                schema: "public",
                table: "TicketTypes");

            migrationBuilder.DropColumn(
                name: "UsedAt",
                schema: "public",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "CheckIns");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "CheckIns");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "CheckIns");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "CheckIns");

            migrationBuilder.DropColumn(
                name: "EventSectionId",
                schema: "public",
                table: "BookingItems");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "VenueSections",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "VenueSections",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                schema: "public",
                table: "VenueSections",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultBasePrice",
                schema: "public",
                table: "VenueSections",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "VenueSections",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Venues",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsIndoor",
                schema: "public",
                table: "Venues",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAccessible",
                schema: "public",
                table: "Venues",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "public",
                table: "Venues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Venues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "public",
                table: "Venues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine1",
                schema: "public",
                table: "Venues",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "public",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "SoldQuantity",
                schema: "public",
                table: "TicketTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleStartDate",
                schema: "public",
                table: "TicketTypes",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleEndDate",
                schema: "public",
                table: "TicketTypes",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                schema: "public",
                table: "TicketTypes",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "TicketTypes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TicketCode",
                schema: "public",
                table: "Tickets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "IssuedAt",
                schema: "public",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "public",
                table: "Reviews",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "public",
                table: "Notifications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "public",
                table: "Notifications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "public",
                table: "EventSessions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "EventSessions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "EventSections",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "EventSections",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "EventSections",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "public",
                table: "EventSections",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "public",
                table: "Events",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                schema: "public",
                table: "Events",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizerName",
                schema: "public",
                table: "Events",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MinTicketsPerOrder",
                schema: "public",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "MaxTicketsPerOrder",
                schema: "public",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 10,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublished",
                schema: "public",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFree",
                schema: "public",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "Events",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                schema: "public",
                table: "Events",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BannerImageUrl",
                schema: "public",
                table: "Events",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VenueId1",
                schema: "public",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "EventCategories",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "EventCategories",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                schema: "public",
                table: "EventCategories",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                schema: "public",
                table: "EventCategories",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "EventCategories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                schema: "public",
                table: "CheckIns",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                schema: "public",
                table: "Bookings",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                schema: "public",
                table: "Bookings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                schema: "public",
                table: "BookingItems",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateIndex(
                name: "IX_VenueSections_TenantId",
                schema: "public",
                table: "VenueSections",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_TenantId",
                schema: "public",
                table: "Venues",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_TenantId",
                schema: "public",
                table: "TicketTypes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TenantId",
                schema: "public",
                table: "Tickets",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketCode",
                schema: "public",
                table: "Tickets",
                column: "TicketCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_TenantId",
                schema: "public",
                table: "EventSessions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSections_TenantId",
                schema: "public",
                table: "EventSections",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_TenantId",
                schema: "public",
                table: "Events",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_VenueId1",
                schema: "public",
                table: "Events",
                column: "VenueId1");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_TenantId",
                schema: "public",
                table: "EventCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_TenantId_Name",
                schema: "public",
                table: "EventCategories",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_TicketId",
                schema: "public",
                table: "CheckIns",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TenantId",
                schema: "public",
                table: "Bookings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_TenantId",
                schema: "public",
                table: "BookingItems",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_TicketTypes_TicketTypeId",
                schema: "public",
                table: "BookingItems",
                column: "TicketTypeId",
                principalSchema: "public",
                principalTable: "TicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Events_EventId",
                schema: "public",
                table: "Bookings",
                column: "EventId",
                principalSchema: "public",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                schema: "public",
                table: "Bookings",
                column: "UserId",
                principalSchema: "public",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventCategories_EventCategoryId",
                schema: "public",
                table: "Events",
                column: "EventCategoryId",
                principalSchema: "public",
                principalTable: "EventCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venues_VenueId",
                schema: "public",
                table: "Events",
                column: "VenueId",
                principalSchema: "public",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venues_VenueId1",
                schema: "public",
                table: "Events",
                column: "VenueId1",
                principalSchema: "public",
                principalTable: "Venues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventSections_VenueSections_VenueSectionId",
                schema: "public",
                table: "EventSections",
                column: "VenueSectionId",
                principalSchema: "public",
                principalTable: "VenueSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventSessions_Speakers_SpeakerId",
                schema: "public",
                table: "EventSessions",
                column: "SpeakerId",
                principalSchema: "public",
                principalTable: "Speakers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
