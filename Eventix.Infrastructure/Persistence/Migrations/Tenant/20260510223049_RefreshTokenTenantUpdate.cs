using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Persistence.Migrations.Tenant
{
    /// <inheritdoc />
    public partial class RefreshTokenTenantUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_TenantId_UserId_CreatedAt",
                schema: "public",
                table: "AuditLog");

            migrationBuilder.DropIndex(
                name: "IX_AIRequestLogs_TenantId_UserId_CreatedAt",
                schema: "public",
                table: "AIRequestLog");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "AIRequestLog");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine1",
                schema: "public",
                table: "Venue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<Guid>(
                name: "PublicUserId",
                schema: "public",
                table: "User",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGlobal",
                schema: "public",
                table: "Role",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicUserId",
                schema: "public",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsGlobal",
                schema: "public",
                table: "Role");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Venue",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "public",
                table: "Venue",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Venue",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "public",
                table: "Venue",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine1",
                schema: "public",
                table: "Venue",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "AuditLog",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "AIRequestLog",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_UserId_CreatedAt",
                schema: "public",
                table: "AuditLog",
                columns: new[] { "TenantId", "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AIRequestLogs_TenantId_UserId_CreatedAt",
                schema: "public",
                table: "AIRequestLog",
                columns: new[] { "TenantId", "UserId", "CreatedAt" });
        }
    }
}

