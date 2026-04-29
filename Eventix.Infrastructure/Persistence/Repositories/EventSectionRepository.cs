using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
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

    private Guid TenantId => _tenantContext.TenantId;

    public async Task<IReadOnlyList<EventSection>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.EventSections
            .AsNoTracking()
            .Include(x => x.Event)
            .Include(x => x.VenueSection)
            .Where(x => x.TenantId == TenantId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<EventSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.EventSections
            .Include(x => x.Event)
            .Include(x => x.VenueSection)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.TenantId == TenantId &&
                !x.IsDeleted,
                cancellationToken);

    public async Task<IReadOnlyList<EventSection>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
        => await _context.EventSections
            .AsNoTracking()
            .Include(x => x.VenueSection)
            .Where(x =>
                x.EventId == eventId &&
                x.TenantId == TenantId &&
                !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsByEventAndVenueSectionAsync(
        Guid eventId,
        Guid venueSectionId,
        CancellationToken cancellationToken = default)
        => _context.EventSections.AnyAsync(x =>
            x.EventId == eventId &&
            x.VenueSectionId == venueSectionId &&
            x.TenantId == TenantId &&
            !x.IsDeleted,
            cancellationToken);

    public async Task AddAsync(EventSection entity, CancellationToken cancellationToken = default)
    {
        entity.TenantId = TenantId;
        await _context.EventSections.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(EventSection entity)
    {
        _context.EventSections.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(EventSection entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}