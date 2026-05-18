using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventix.Infrastructure.Persistence.Migrations.Public
{
    /// <inheritdoc />
    public partial class RepairTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
    ALTER TABLE public."Tenants"
    ADD COLUMN IF NOT EXISTS "IsTrial" boolean NOT NULL DEFAULT false;

    ALTER TABLE public."Tenants"
    ADD COLUMN IF NOT EXISTS "Description" character varying(500);

    ALTER TABLE public."Tenants"
    ADD COLUMN IF NOT EXISTS "ContactEmail" character varying(200);

    ALTER TABLE public."Tenants"
    ADD COLUMN IF NOT EXISTS "City" character varying(100);

    ALTER TABLE public."Tenants"
    ADD COLUMN IF NOT EXISTS "Country" character varying(100);

    ALTER TABLE public."Tenants"
    ADD COLUMN IF NOT EXISTS "LogoUrl" character varying(500);

    ALTER TABLE public."Tenants"
    ADD COLUMN IF NOT EXISTS "Status" integer NOT NULL DEFAULT 1;

    ALTER TABLE public."Tenants"
    ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT false;

    ALTER TABLE public."Tenants"
    ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT true;

    ALTER TABLE public."Tenants"
    ALTER COLUMN "IsActive" SET DEFAULT true;

""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}