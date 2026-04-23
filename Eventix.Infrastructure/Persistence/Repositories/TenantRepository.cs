using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly PublicDbContext _context;

    public TenantRepository(PublicDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Tenants
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Tenants
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        => await _context.Tenants
            .FirstOrDefaultAsync(x => x.Slug == slug && !x.IsDeleted, cancellationToken);

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
        => await _context.Tenants
            .AnyAsync(x => x.Slug == slug && !x.IsDeleted, cancellationToken);

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _context.Tenants.AddAsync(tenant, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        tenant.IsDeleted = true;
        tenant.UpdatedAtUtc = DateTime.UtcNow;
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken);
    }
}