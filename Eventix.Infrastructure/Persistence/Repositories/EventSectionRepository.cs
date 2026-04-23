using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class EventSectionRepository : IEventSectionRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenantContext;

    public EventSectionRepository(TenantDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<EventSection>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.EventSections
            .AsNoTracking()
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<EventSection>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        => await _context.EventSections
            .AsNoTracking()
            .Where(x => x.TenantId == _tenantContext.TenantId &&
                        x.EventId == eventId &&
                        !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<EventSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.EventSections
            .FirstOrDefaultAsync(x => x.Id == id &&
                                      x.TenantId == _tenantContext.TenantId &&
                                      !x.IsDeleted, cancellationToken);

    public async Task<bool> ExistsByEventAndVenueSectionAsync(Guid eventId, Guid venueSectionId, CancellationToken cancellationToken = default)
        => await _context.EventSections
            .AnyAsync(x => x.TenantId == _tenantContext.TenantId &&
                           x.EventId == eventId &&
                           x.VenueSectionId == venueSectionId &&
                           !x.IsDeleted, cancellationToken);

    public async Task AddAsync(EventSection entity, CancellationToken cancellationToken = default)
    {
        entity.TenantId = _tenantContext.TenantId;
        await _context.EventSections.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(EventSection entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAtUtc = DateTime.UtcNow;
        _context.EventSections.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(EventSection entity, CancellationToken cancellationToken = default)
    {
        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        _context.EventSections.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}