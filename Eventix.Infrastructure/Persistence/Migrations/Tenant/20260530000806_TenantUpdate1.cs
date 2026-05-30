using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Persistence.Migrations.Tenant
{
    /// <inheritdoc />
    public partial class TenantUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLog_User_UserId",
                schema: "public",
                table: "AuditLog");

            migrationBuilder.DropIndex(
                name: "IX_AuditLog_TenantId_EntityName_EntityId",
                schema: "public",
                table: "AuditLog");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "public",
                table: "AuditLog",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "AuditLog",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "OldValues",
                schema: "public",
                table: "AuditLog",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewValues",
                schema: "public",
                table: "AuditLog",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntityName",
                schema: "public",
                table: "AuditLog",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "TenantName",
                schema: "public",
                table: "AuditLog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                schema: "public",
                table: "AuditLog",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLog_User_UserId",
                schema: "public",
                table: "AuditLog",
                column: "UserId",
                principalSchema: "public",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLog_User_UserId",
                schema: "public",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "TenantName",
                schema: "public",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                schema: "public",
                table: "AuditLog");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "public",
                table: "AuditLog",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                schema: "public",
                table: "AuditLog",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OldValues",
                schema: "public",
                table: "AuditLog",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewValues",
                schema: "public",
                table: "AuditLog",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntityName",
                schema: "public",
                table: "AuditLog",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_TenantId_EntityName_EntityId",
                schema: "public",
                table: "AuditLog",
                columns: new[] { "TenantId", "EntityName", "EntityId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLog_User_UserId",
                schema: "public",
                table: "AuditLog",
                column: "UserId",
                principalSchema: "public",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
