using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Persistence.Migrations.Public
{
    /// <inheritdoc />
    public partial class AddTenantEmailDomains : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantEmailDomains",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Domain = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DefaultRoleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AutoApprove = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantEmailDomains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantEmailDomains_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "public",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantEmailDomains_TenantId_Domain",
                schema: "public",
                table: "TenantEmailDomains",
                columns: new[] { "TenantId", "Domain" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantEmailDomains",
                schema: "public");
        }
    }
}
