using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.MultiTenancy;

public class TenantSchemaProvisioner : ITenantSchemaProvisioner
{
    private readonly TenantDbContext _tenantDbContext;
    private readonly ITenantContext _tenantContext;

    public TenantSchemaProvisioner(
        TenantDbContext tenantDbContext,
        ITenantContext tenantContext)
    {
        _tenantDbContext = tenantDbContext;
        _tenantContext = tenantContext;
    }

    public async Task ProvisionTenantSchemaAsync(
        string schemaName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
            throw new ArgumentException("Schema name is required.", nameof(schemaName));

        _tenantContext.SchemaName = schemaName;

        await _tenantDbContext.Database.ExecuteSqlRawAsync(
            $@"CREATE SCHEMA IF NOT EXISTS ""{schemaName}"";",
            cancellationToken
        );

        await _tenantDbContext.Database.MigrateAsync(cancellationToken);
    }
}