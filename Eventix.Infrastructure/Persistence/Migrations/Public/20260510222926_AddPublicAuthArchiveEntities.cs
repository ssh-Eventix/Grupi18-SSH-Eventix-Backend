using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Persistence.Migrations.Public
{
    /// <inheritdoc />
    public partial class AddPublicAuthArchiveEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantImpersonationLogs_ImpersonatorPublicUserId",
                schema: "public",
                table: "TenantImpersonationLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PublicUserRoles",
                schema: "public",
                table: "PublicUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_PublicUserRoles_PublicUserId_PublicRoleId",
                schema: "public",
                table: "PublicUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_PublicRoles_Name",
                schema: "public",
                table: "PublicRoles");

            migrationBuilder.DropColumn(
                name: "ImpersonatorPublicUserId",
                schema: "public",
                table: "TenantImpersonationLogs");

            migrationBuilder.DropColumn(
                name: "IsSuperAdmin",
                schema: "public",
                table: "PublicUsers");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "PublicUserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "PublicUserRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "PublicUserRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "PublicUserRoles");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                schema: "public",
                table: "TenantImpersonationLogs",
                newName: "TargetTenantId");

            migrationBuilder.RenameColumn(
                name: "TargetTenantUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                newName: "SuperAdminUserId");

            migrationBuilder.RenameColumn(
                name: "ImpersonatorTenantUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                newName: "TargetUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantImpersonationLogs_TenantId",
                schema: "public",
                table: "TenantImpersonationLogs",
                newName: "IX_TenantImpersonationLogs_TargetTenantId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantImpersonationLogs_ImpersonatorTenantUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                newName: "IX_TenantImpersonationLogs_TargetUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                schema: "public",
                table: "TenantImpersonationLogs",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Event",
                schema: "public",
                table: "TenantImpersonationLogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                schema: "public",
                table: "PublicUsers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAtUtc",
                schema: "public",
                table: "PublicUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                schema: "public",
                table: "PublicRoles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PublicUserRoles",
                schema: "public",
                table: "PublicUserRoles",
                columns: new[] { "PublicUserId", "PublicRoleId" });

            migrationBuilder.CreateTable(
                name: "ArchiveRecords",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchemaName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArchiveYear = table.Column<int>(type: "integer", nullable: false),
                    DataJson = table.Column<string>(type: "jsonb", nullable: false),
                    ArchivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReplacedByTokenHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_PublicUsers_PublicUserId",
                        column: x => x.PublicUserId,
                        principalSchema: "public",
                        principalTable: "PublicUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantImpersonationLogs_SuperAdminUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                column: "SuperAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantImpersonationLogs_TargetTenantId_IsActive",
                schema: "public",
                table: "TenantImpersonationLogs",
                columns: new[] { "TargetTenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicRoles_NormalizedName",
                schema: "public",
                table: "PublicRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveRecords_ArchiveYear",
                schema: "public",
                table: "ArchiveRecords",
                column: "ArchiveYear");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveRecords_SchemaName",
                schema: "public",
                table: "ArchiveRecords",
                column: "SchemaName");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveRecords_TenantId",
                schema: "public",
                table: "ArchiveRecords",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveRecords_TenantId_EntityName_EntityId",
                schema: "public",
                table: "ArchiveRecords",
                columns: new[] { "TenantId", "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_PublicUserId",
                schema: "public",
                table: "RefreshTokens",
                column: "PublicUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                schema: "public",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantImpersonationLogs_PublicUsers_SuperAdminUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                column: "SuperAdminUserId",
                principalSchema: "public",
                principalTable: "PublicUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantImpersonationLogs_PublicUsers_TargetUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                column: "TargetUserId",
                principalSchema: "public",
                principalTable: "PublicUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantImpersonationLogs_Tenants_TargetTenantId",
                schema: "public",
                table: "TenantImpersonationLogs",
                column: "TargetTenantId",
                principalSchema: "public",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantImpersonationLogs_PublicUsers_SuperAdminUserId",
                schema: "public",
                table: "TenantImpersonationLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantImpersonationLogs_PublicUsers_TargetUserId",
                schema: "public",
                table: "TenantImpersonationLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantImpersonationLogs_Tenants_TargetTenantId",
                schema: "public",
                table: "TenantImpersonationLogs");

            migrationBuilder.DropTable(
                name: "ArchiveRecords",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_TenantImpersonationLogs_SuperAdminUserId",
                schema: "public",
                table: "TenantImpersonationLogs");

            migrationBuilder.DropIndex(
                name: "IX_TenantImpersonationLogs_TargetTenantId_IsActive",
                schema: "public",
                table: "TenantImpersonationLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PublicUserRoles",
                schema: "public",
                table: "PublicUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_PublicRoles_NormalizedName",
                schema: "public",
                table: "PublicRoles");

            migrationBuilder.DropColumn(
                name: "Event",
                schema: "public",
                table: "TenantImpersonationLogs");

            migrationBuilder.DropColumn(
                name: "LastLoginAtUtc",
                schema: "public",
                table: "PublicUsers");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                schema: "public",
                table: "PublicRoles");

            migrationBuilder.RenameColumn(
                name: "TargetUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                newName: "ImpersonatorTenantUserId");

            migrationBuilder.RenameColumn(
                name: "TargetTenantId",
                schema: "public",
                table: "TenantImpersonationLogs",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "SuperAdminUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                newName: "TargetTenantUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantImpersonationLogs_TargetUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                newName: "IX_TenantImpersonationLogs_ImpersonatorTenantUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantImpersonationLogs_TargetTenantId",
                schema: "public",
                table: "TenantImpersonationLogs",
                newName: "IX_TenantImpersonationLogs_TenantId");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                schema: "public",
                table: "TenantImpersonationLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImpersonatorPublicUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                schema: "public",
                table: "PublicUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdmin",
                schema: "public",
                table: "PublicUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "PublicUserRoles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "public",
                table: "PublicUserRoles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "PublicUserRoles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "PublicUserRoles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PublicUserRoles",
                schema: "public",
                table: "PublicUserRoles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TenantImpersonationLogs_ImpersonatorPublicUserId",
                schema: "public",
                table: "TenantImpersonationLogs",
                column: "ImpersonatorPublicUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicUserRoles_PublicUserId_PublicRoleId",
                schema: "public",
                table: "PublicUserRoles",
                columns: new[] { "PublicUserId", "PublicRoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicRoles_Name",
                schema: "public",
                table: "PublicRoles",
                column: "Name",
                unique: true);
        }
    }
}
