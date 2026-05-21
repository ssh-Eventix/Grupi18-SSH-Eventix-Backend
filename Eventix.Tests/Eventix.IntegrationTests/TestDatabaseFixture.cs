using Eventix.Application.Interfaces.Common;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

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
}