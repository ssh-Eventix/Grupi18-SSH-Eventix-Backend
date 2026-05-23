using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Application.Services;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Eventix.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Eventix.IntegrationTests;

public class TestDatabaseFixture : IAsyncLifetime
{
    public string ConnectionString { get; } =
        "Host=localhost;Port=5432;Database=eventix_test;Username=postgres;Password=BORVYdb2026SSH!";

    public ServiceProvider Services { get; private set; } = null!;

    public Task InitializeAsync()
    {
        var services = new ServiceCollection();

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<TenantSchemaProvisioner>();

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventService, EventService>();
        services.AddDbContext<PublicDbContext>(options =>
        {
            options.UseNpgsql(ConnectionString);
        });

        services.AddScoped<TenantEmailDomainRepository>();
        services.AddDbContext<TenantDbContext>((sp, options) =>
        {
            options.UseNpgsql(ConnectionString);

            options.ReplaceService<
                IModelCacheKeyFactory,
                TenantModelCacheKeyFactory>();
        });

        Services = services.BuildServiceProvider();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Services.DisposeAsync();
    }

    public async Task DropSchemaAsync(string schemaName)
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            $@"DROP SCHEMA IF EXISTS ""{schemaName}"" CASCADE;",
            conn);

        await cmd.ExecuteNonQueryAsync();
    }
}