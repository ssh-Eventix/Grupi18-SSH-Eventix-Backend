using Eventix.Application.Interfaces.Services;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

public class TenantSchemaProvisioner : ITenantSchemaProvisioner
{
    private readonly TenantDbContext _tenantDbContext;

    public TenantSchemaProvisioner(TenantDbContext tenantDbContext)
    {
        _tenantDbContext = tenantDbContext;
    }

    public async Task ProvisionTenantSchemaAsync(string schemaName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
            throw new ArgumentException("Schema name is required.", nameof(schemaName));

        await _tenantDbContext.Database.ExecuteSqlRawAsync(
            $@"CREATE SCHEMA IF NOT EXISTS ""{schemaName}"";",
            cancellationToken
        );

        await _tenantDbContext.Database.MigrateAsync(cancellationToken);
    }
}