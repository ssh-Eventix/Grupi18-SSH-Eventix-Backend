using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Eventix.Application.Interfaces.Common;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class EventCategoryRepository : IEventCategoryRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenantContext;

    public EventCategoryRepository(TenantDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public Task<List<EventCategory>> GetAllAsync(CancellationToken cancellationToken = default)
        => _context.EventCategories
            .AsNoTracking()
            .Include(x => x.Events)
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public Task<EventCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.EventCategories
            .Include(x => x.Events)
            .FirstOrDefaultAsync(x => x.Id == id &&
                                      x.TenantId == _tenantContext.TenantId &&
                                      !x.IsDeleted, cancellationToken);

    public async Task AddAsync(EventCategory entity, CancellationToken cancellationToken = default)
        => await _context.EventCategories.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(EventCategory entity)
    {
        _context.EventCategories.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(EventCategory entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}