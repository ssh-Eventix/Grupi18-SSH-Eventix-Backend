using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PublicUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicRoles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicUsers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    IsSuperAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantImpersonationLogs",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImpersonatorPublicUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImpersonatorTenantUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetTenantUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantImpersonationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicUserRoles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicUserRoles_PublicRoles_PublicRoleId",
                        column: x => x.PublicRoleId,
                        principalSchema: "public",
                        principalTable: "PublicRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublicUserRoles_PublicUsers_PublicUserId",
                        column: x => x.PublicUserId,
                        principalSchema: "public",
                        principalTable: "PublicUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublicRoles_Name",
                schema: "public",
                table: "PublicRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicUserRoles_PublicRoleId",
                schema: "public",
                table: "PublicUserRoles",
                column: "PublicRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicUserRoles_PublicUserId_PublicRoleId",
                schema: "public",
                table: "PublicUserRoles",
                columns: new[] { "PublicUserId", "PublicRoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicUsers_Email",
                schema: "public",
                table: "PublicUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantImpersonationLogs_ImpersonatorPublicUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                column: "ImpersonatorPublicUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantImpersonationLogs_ImpersonatorTenantUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                column: "ImpersonatorTenantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantImpersonationLogs_TenantId",
                schema: "public",
                table: "TenantImpersonationLogs",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicUserRoles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TenantImpersonationLogs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PublicRoles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PublicUsers",
                schema: "public");
        }
    }
}
