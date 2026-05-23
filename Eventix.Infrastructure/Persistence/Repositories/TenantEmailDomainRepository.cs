using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class TenantEmailDomainRepository : ITenantEmailDomainRepository
{
    private readonly PublicDbContext _context;

    public TenantEmailDomainRepository(PublicDbContext context)
    {
        _context = context;
    }
    public Task<List<TenantEmailDomain>> GetAllAsync(CancellationToken ct)
    {
        return _context.TenantEmailDomains
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);
    }
    public Task<List<TenantEmailDomain>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct)
    {
        return _context.TenantEmailDomains
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .ToListAsync(ct);
    }
    public Task<TenantEmailDomain?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _context.TenantEmailDomains
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
    }
    public Task<TenantEmailDomain?> GetByDomainAsync(string domain, CancellationToken ct)
    {
        var normalized = domain.Trim().ToLower();

        return _context.TenantEmailDomains
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Domain == normalized && !x.IsDeleted, ct);
    }
    public async Task AddAsync(TenantEmailDomain entity, CancellationToken ct)
    {
        await _context.TenantEmailDomains.AddAsync(entity, ct);
    }
    public Task UpdateAsync(TenantEmailDomain entity, CancellationToken ct)
    {
        _context.TenantEmailDomains.Update(entity);
        return Task.CompletedTask;
    }
    public Task DeleteAsync(TenantEmailDomain entity, CancellationToken ct)
    {
        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        _context.TenantEmailDomains.Update(entity);
        return Task.CompletedTask;
    }
    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
    public Task<TenantEmailDomain?> GetByTenantIdAndDomainAsync(
        Guid tenantId,
        string domain,
        CancellationToken ct)
    {
        return _context.TenantEmailDomains
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.Domain == domain.ToLower(),
                ct);
    }
}