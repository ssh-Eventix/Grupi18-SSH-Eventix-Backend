using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.Infrastructure.MultiTenancy;

public class TenantSchemaProvisioner : ITenantSchemaProvisioner
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TenantSchemaProvisioner(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ProvisionTenantSchemaAsync(
    string schemaName,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
            throw new ArgumentException("Schema name is required.", nameof(schemaName));

        await using var scope = _scopeFactory.CreateAsyncScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.SchemaName = schemaName;

        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        await db.Database.ExecuteSqlRawAsync(
            $@"DROP SCHEMA IF EXISTS ""{schemaName}"" CASCADE;",
            cancellationToken);

        await db.Database.ExecuteSqlRawAsync(
            $@"CREATE SCHEMA ""{schemaName}"";",
            cancellationToken);

        var createScript = db.Database.GenerateCreateScript();

        await db.Database.ExecuteSqlRawAsync(
            $@"SET search_path TO ""{schemaName}"";
       {createScript}",
            cancellationToken);
    }
}