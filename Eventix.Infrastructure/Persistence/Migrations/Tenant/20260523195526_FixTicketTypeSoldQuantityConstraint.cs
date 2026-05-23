using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Persistence.Migrations.Tenant
{
    /// <inheritdoc />
    public partial class FixTicketTypeSoldQuantityConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketType_SoldQuantity_Limit",
                schema: "public",
                table: "TicketType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketType_SoldQuantity_Limit",
                schema: "public",
                table: "TicketType",
                sql: "\"SoldQuantity\" <= \"QuantityAvailable\"");
        }
    }
}
