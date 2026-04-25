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

    public async Task<IReadOnlyList<EventSection>> GetAllAsync(CancellationToken ct = default)
        => await _context.EventSections
            .AsNoTracking()
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .ToListAsync(ct);

    public async Task<EventSection?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.EventSections
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.TenantId == _tenantContext.TenantId &&
                !x.IsDeleted, ct);

    public async Task<IReadOnlyList<EventSection>> GetByEventIdAsync(
    Guid eventId,
    CancellationToken ct = default)
    {
        return await _context.EventSections
            .AsNoTracking()
            .Where(x =>
                x.EventId == eventId &&
                x.TenantId == _tenantContext.TenantId &&
                !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task AddAsync(EventSection entity, CancellationToken ct = default)
    {
        entity.TenantId = _tenantContext.TenantId;
        await _context.EventSections.AddAsync(entity, ct);
    }

    public Task UpdateAsync(EventSection entity)
    {
        _context.EventSections.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(EventSection entity)
    {
        entity.IsDeleted = true;
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByEventAndVenueSectionAsync(Guid eventId, Guid venueSectionId, CancellationToken ct = default)
        => _context.EventSections.AnyAsync(x =>
            x.EventId == eventId &&
            x.VenueSectionId == venueSectionId &&
            x.TenantId == _tenantContext.TenantId &&
            !x.IsDeleted, ct);
}