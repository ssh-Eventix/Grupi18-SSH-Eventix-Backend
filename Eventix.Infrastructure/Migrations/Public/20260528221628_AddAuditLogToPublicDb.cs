using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Migrations.Public
{
    /// <inheritdoc />
    public partial class AddAuditLogToPublicDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                 name: "TenantName",
                 schema: "public",
                 table: "AuditLog",
                 type: "character varying(200)",
                 maxLength: 200,
                 nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                schema: "public",
                table: "AuditLog",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantName",
                schema: "public",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                schema: "public",
                table: "AuditLog");
        }
    }
}
