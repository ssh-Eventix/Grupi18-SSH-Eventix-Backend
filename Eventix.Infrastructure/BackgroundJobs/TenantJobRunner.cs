using Eventix.Application.Interfaces.Common;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.Infrastructure.BackgroundJobs;

public class TenantJobRunner
{
    private readonly PublicDbContext _publicDbContext;
    private readonly IServiceScopeFactory _scopeFactory;

    public TenantJobRunner(
        PublicDbContext publicDbContext,
        IServiceScopeFactory scopeFactory)
    {
        _publicDbContext = publicDbContext;
        _scopeFactory = scopeFactory;
    }

    public async Task RunForAllTenants(Func<IServiceProvider, Task> job)
    {
        var tenants = await _publicDbContext.Tenants
            .Where(x => x.IsActive)
            .ToListAsync();

        foreach (var tenant in tenants)
        {
            using var scope = _scopeFactory.CreateScope();

            var tenantContext =
                scope.ServiceProvider.GetRequiredService<ITenantContext>();

            tenantContext.TenantId = tenant.Id;
            tenantContext.SchemaName = tenant.SchemaName;

            await job(scope.ServiceProvider);
        }
    }
}