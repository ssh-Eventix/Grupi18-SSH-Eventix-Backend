using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.Infrastructure.Services;

public class TenantRoleSeeder : ITenantRoleSeeder
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TenantRoleSeeder(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task SeedDefaultRolesAsync(
        Guid tenantId,
        string schemaName,
        CancellationToken ct = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.TenantId = tenantId;
        tenantContext.SchemaName = schemaName;

        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        var existingRoles = await db.Roles
            .Where(x => x.TenantId == tenantId)
            .Select(x => x.Name)
            .ToListAsync(ct);

        var roles = new List<Role>();

        if (!existingRoles.Contains("Admin"))
        {
            roles.Add(new Role
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "Admin",
                Description = "Tenant administrator role",
                IsGlobal = false,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        if (!existingRoles.Contains("Staff"))
        {
            roles.Add(new Role
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "Staff",
                Description = "Tenant staff role",
                IsGlobal = false,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        if (!existingRoles.Contains("Buyer"))
        {
            roles.Add(new Role
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "Buyer",
                Description = "Buyer role",
                IsGlobal = false,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        if (roles.Count > 0)
        {
            await db.Roles.AddRangeAsync(roles, ct);
            await db.SaveChangesAsync(ct);
        }
    }
}