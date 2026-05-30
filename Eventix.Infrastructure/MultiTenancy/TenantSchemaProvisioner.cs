using System.Text.RegularExpressions;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.Infrastructure.MultiTenancy;

public class TenantSchemaProvisioner : ITenantSchemaProvisioner
{
    private readonly IServiceScopeFactory _scopeFactory;

    private static readonly Regex SchemaRegex =
        new(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

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

        if (!SchemaRegex.IsMatch(schemaName))
            throw new ArgumentException("Invalid schema name.", nameof(schemaName));

        await using var scope = _scopeFactory.CreateAsyncScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.SchemaName = schemaName;

        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

#pragma warning disable EF1002

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

#pragma warning restore EF1002
    }
}