using Eventix.Application.Interfaces.Services;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.MultiTenancy;

public class TenantSchemaProvisioner : ITenantSchemaProvisioner
{
    private readonly PublicDbContext _publicDbContext;

    public TenantSchemaProvisioner(PublicDbContext publicDbContext)
    {
        _publicDbContext = publicDbContext;
    }

    public async Task ProvisionTenantSchemaAsync(string schemaName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
            throw new ArgumentException("Schema name is required.", nameof(schemaName));

        var sql = $@"
CREATE SCHEMA IF NOT EXISTS ""{schemaName}"";

CREATE TABLE IF NOT EXISTS ""{schemaName}"".""EventCategories"" (
    ""Id"" uuid PRIMARY KEY,
    ""TenantId"" uuid NOT NULL,
    ""Name"" varchar(150) NOT NULL,
    ""Description"" varchar(500) NULL,
    ""CreatedAtUtc"" timestamp without time zone NOT NULL,
    ""UpdatedAtUtc"" timestamp without time zone NULL
);

CREATE TABLE IF NOT EXISTS ""{schemaName}"".""Venues"" (
    ""Id"" uuid PRIMARY KEY,
    ""TenantId"" uuid NOT NULL,
    ""Name"" varchar(200) NOT NULL,
    ""Address"" varchar(300) NOT NULL,
    ""City"" varchar(100) NOT NULL,
    ""Country"" varchar(100) NOT NULL,
    ""Capacity"" integer NOT NULL,
    ""CreatedAtUtc"" timestamp without time zone NOT NULL,
    ""UpdatedAtUtc"" timestamp without time zone NULL
);

CREATE TABLE IF NOT EXISTS ""{schemaName}"".""VenueSections"" (
    ""Id"" uuid PRIMARY KEY,
    ""TenantId"" uuid NOT NULL,
    ""VenueId"" uuid NOT NULL,
    ""Name"" varchar(150) NOT NULL,
    ""Capacity"" integer NOT NULL,
    ""Description"" varchar(500) NULL,
    ""CreatedAtUtc"" timestamp without time zone NOT NULL,
    ""UpdatedAtUtc"" timestamp without time zone NULL,
    CONSTRAINT ""FK_VenueSections_Venues_VenueId""
        FOREIGN KEY (""VenueId"") REFERENCES ""{schemaName}"".""Venues""(""Id"")
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ""{schemaName}"".""Events"" (
    ""Id"" uuid PRIMARY KEY,
    ""TenantId"" uuid NOT NULL,
    ""OrganizerId"" uuid NOT NULL,
    ""CategoryId"" uuid NOT NULL,
    ""VenueId"" uuid NOT NULL,
    ""Title"" varchar(200) NOT NULL,
    ""Description"" varchar(2000) NULL,
    ""StartDateUtc"" timestamp without time zone NOT NULL,
    ""EndDateUtc"" timestamp without time zone NOT NULL,
    ""Status"" varchar(50) NOT NULL,
    ""CreatedAtUtc"" timestamp without time zone NOT NULL,
    ""UpdatedAtUtc"" timestamp without time zone NULL,
    CONSTRAINT ""FK_Events_EventCategories_CategoryId""
        FOREIGN KEY (""CategoryId"") REFERENCES ""{schemaName}"".""EventCategories""(""Id"")
        ON DELETE RESTRICT,
    CONSTRAINT ""FK_Events_Venues_VenueId""
        FOREIGN KEY (""VenueId"") REFERENCES ""{schemaName}"".""Venues""(""Id"")
        ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS ""IX_EventCategories_Name""
    ON ""{schemaName}"".""EventCategories"" (""Name"");

CREATE INDEX IF NOT EXISTS ""IX_VenueSections_VenueId""
    ON ""{schemaName}"".""VenueSections"" (""VenueId"");

CREATE INDEX IF NOT EXISTS ""IX_Events_CategoryId""
    ON ""{schemaName}"".""Events"" (""CategoryId"");

CREATE INDEX IF NOT EXISTS ""IX_Events_VenueId""
    ON ""{schemaName}"".""Events"" (""VenueId"");

CREATE INDEX IF NOT EXISTS ""IX_Events_Title""
    ON ""{schemaName}"".""Events"" (""Title"");
";

        await _publicDbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }
}