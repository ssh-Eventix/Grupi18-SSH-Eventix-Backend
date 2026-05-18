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
            $@"CREATE SCHEMA IF NOT EXISTS ""{schemaName}"";",
            cancellationToken);

        var createScript = db.Database.GenerateCreateScript();

        await db.Database.ExecuteSqlRawAsync(createScript, cancellationToken);

        var roleTableExists = await db.Database
            .SqlQueryRaw<int>(
                $@"SELECT CASE WHEN to_regclass('""{schemaName}"".""Role""') IS NULL THEN 0 ELSE 1 END AS ""Value""")
            .SingleAsync(cancellationToken);

        if (roleTableExists == 0)
            throw new InvalidOperationException($@"Tenant schema was created, but ""{schemaName}"".""Role"" table was not created.");
    }
}