using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventSessions_Event_EventId",
                schema: "public",
                table: "EventSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSessions_Speakers_SpeakerId",
                schema: "public",
                table: "EventSessions");

            migrationBuilder.DropTable(
                name: "CheckIns",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "public");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Speakers",
                schema: "public",
                table: "Speakers");

            migrationBuilder.DropIndex(
                name: "IX_Speakers_Email",
                schema: "public",
                table: "Speakers");

            migrationBuilder.DropIndex(
                name: "IX_Speakers_TenantId",
                schema: "public",
                table: "Speakers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventSessions",
                schema: "public",
                table: "EventSessions");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "VenueSection");

            migrationBuilder.DropColumn(
                name: "IsAccessibleSection",
                schema: "public",
                table: "VenueSection");

            migrationBuilder.DropColumn(
                name: "RowCount",
                schema: "public",
                table: "VenueSection");

            migrationBuilder.DropColumn(
                name: "SeatsPerRow",
                schema: "public",
                table: "VenueSection");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "HasSeatingMap",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "SeatingMapImageUrl",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "State",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "public",
                table: "Venue");

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
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

            migrationBuilder.DropColumn(
                name: "Benefits",
                schema: "public",
                table: "EventSection");

            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "public",
                table: "EventSection");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                schema: "public",
                table: "EventSection");

            migrationBuilder.DropColumn(
                name: "MaxTicketsPerOrder",
                schema: "public",
                table: "EventSection");

            migrationBuilder.DropColumn(
                name: "MinTicketsPerOrder",
                schema: "public",
                table: "EventSection");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "public",
                table: "EventSection");

            migrationBuilder.DropColumn(
                name: "ReservedSeats",
                schema: "public",
                table: "EventSection");

            migrationBuilder.DropColumn(
                name: "SalesEnabled",
                schema: "public",
                table: "EventSection");

            migrationBuilder.DropColumn(
                name: "SoldSeats",
                schema: "public",
                table: "EventSection");

            migrationBuilder.DropColumn(
                name: "AllowWaitlist",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "DoorsOpenUtc",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "MaxPrice",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "MinPrice",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "OrganizerEmail",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "OrganizerPhone",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "RefundPolicy",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "RequiresApproval",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "SalesEndUtc",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "SalesStartUtc",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Subtitle",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Tags",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "TermsAndConditions",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "ThumbnailImageUrl",
                schema: "public",
                table: "Event");

            migrationBuilder.RenameTable(
                name: "Speakers",
                schema: "public",
                newName: "Speaker",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "EventSessions",
                schema: "public",
                newName: "EventSession",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_EventSessions_SpeakerId",
                schema: "public",
                table: "EventSession",
                newName: "IX_EventSession_SpeakerId");

            migrationBuilder.RenameIndex(
                name: "IX_EventSessions_EventId",
                schema: "public",
                table: "EventSession",
                newName: "IX_EventSession_EventId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "public",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "User",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "TicketType",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "Ticket",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Tenants",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                schema: "public",
                table: "Tenants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTrial",
                schema: "public",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "Tenants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "public",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactEmail",
                schema: "public",
                table: "Tenants",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "public",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "BookingItem",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "Booking",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImageUrl",
                schema: "public",
                table: "Speaker",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "public",
                table: "Speaker",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                schema: "public",
                table: "Speaker",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "public",
                table: "Speaker",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                schema: "public",
                table: "Speaker",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "public",
                table: "EventSession",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "public",
                table: "EventSession",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "EventSession",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "EventSession",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "EventSession",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Speaker",
                schema: "public",
                table: "Speaker",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventSession",
                schema: "public",
                table: "EventSession",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
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
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                schema: "public",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                schema: "public",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventSession_Event_EventId",
                schema: "public",
                table: "EventSession",
                column: "EventId",
                principalSchema: "public",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventSession_Speaker_SpeakerId",
                schema: "public",
                table: "EventSession",
                column: "SpeakerId",
                principalSchema: "public",
                principalTable: "Speaker",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventSession_Event_EventId",
                schema: "public",
                table: "EventSession");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSession_Speaker_SpeakerId",
                schema: "public",
                table: "EventSession");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "public");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Speaker",
                schema: "public",
                table: "Speaker");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventSession",
                schema: "public",
                table: "EventSession");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "TicketType");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "BookingItem");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "EventSession");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "EventSession");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "EventSession");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "EventSession");

            migrationBuilder.RenameTable(
                name: "Speaker",
                schema: "public",
                newName: "Speakers",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "EventSession",
                schema: "public",
                newName: "EventSessions",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_EventSession_SpeakerId",
                schema: "public",
                table: "EventSessions",
                newName: "IX_EventSessions_SpeakerId");

            migrationBuilder.RenameIndex(
                name: "IX_EventSession_EventId",
                schema: "public",
                table: "EventSessions",
                newName: "IX_EventSessions_EventId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "VenueSection",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccessibleSection",
                schema: "public",
                table: "VenueSection",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RowCount",
                schema: "public",
                table: "VenueSection",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeatsPerRow",
                schema: "public",
                table: "VenueSection",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasSeatingMap",
                schema: "public",
                table: "Venue",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                schema: "public",
                table: "Venue",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                schema: "public",
                table: "Venue",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeatingMapImageUrl",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "public",
                table: "Venue",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Tenants",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTrial",
                schema: "public",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactEmail",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "public",
                table: "Tenants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

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
                name: "ContactPhone",
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

            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                schema: "public",
                table: "EventSection",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "public",
                table: "EventSection",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                schema: "public",
                table: "EventSection",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxTicketsPerOrder",
                schema: "public",
                table: "EventSection",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinTicketsPerOrder",
                schema: "public",
                table: "EventSection",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "public",
                table: "EventSection",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservedSeats",
                schema: "public",
                table: "EventSection",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SalesEnabled",
                schema: "public",
                table: "EventSection",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SoldSeats",
                schema: "public",
                table: "EventSection",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AllowWaitlist",
                schema: "public",
                table: "Event",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DoorsOpenUtc",
                schema: "public",
                table: "Event",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxPrice",
                schema: "public",
                table: "Event",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinPrice",
                schema: "public",
                table: "Event",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerEmail",
                schema: "public",
                table: "Event",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerPhone",
                schema: "public",
                table: "Event",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundPolicy",
                schema: "public",
                table: "Event",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresApproval",
                schema: "public",
                table: "Event",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SalesEndUtc",
                schema: "public",
                table: "Event",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SalesStartUtc",
                schema: "public",
                table: "Event",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                schema: "public",
                table: "Event",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                schema: "public",
                table: "Event",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                schema: "public",
                table: "Event",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditions",
                schema: "public",
                table: "Event",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImageUrl",
                schema: "public",
                table: "Event",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImageUrl",
                schema: "public",
                table: "Speakers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "public",
                table: "Speakers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                schema: "public",
                table: "Speakers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "public",
                table: "Speakers",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                schema: "public",
                table: "Speakers",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "public",
                table: "EventSessions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Speakers",
                schema: "public",
                table: "Speakers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventSessions",
                schema: "public",
                table: "EventSessions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CheckIns",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckedInByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
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
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_Email",
                schema: "public",
                table: "Speakers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_TenantId",
                schema: "public",
                table: "Speakers",
                column: "TenantId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_EventSessions_Event_EventId",
                schema: "public",
                table: "EventSessions",
                column: "EventId",
                principalSchema: "public",
                principalTable: "Event",
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
        }
    }
}
