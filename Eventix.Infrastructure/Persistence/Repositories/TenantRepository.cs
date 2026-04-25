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

    public Task<List<Tenant>> GetAllAsync(CancellationToken ct)
        => _context.Tenants.AsNoTracking().ToListAsync(ct);

    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct)
        => _context.Tenants.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct)
        => _context.Tenants.FirstOrDefaultAsync(x => x.Slug == slug, ct);

    public async Task AddAsync(Tenant entity, CancellationToken ct)
        => await _context.Tenants.AddAsync(entity, ct);

    public Task UpdateAsync(Tenant entity)
    {
        _context.Tenants.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Tenant entity)
    {
        _context.Tenants.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => _context.SaveChangesAsync(ct);
}