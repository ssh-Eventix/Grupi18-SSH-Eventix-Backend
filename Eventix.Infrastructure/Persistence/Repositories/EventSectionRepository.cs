using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class EventSectionRepository
    : TenantBaseRepository<EventSection>, IEventSectionRepository
{
    public EventSectionRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }

    public override async Task<List<EventSection>> GetAllAsync(CancellationToken ct = default)
    {
        return await Query()
            .AsNoTracking()
            .Include(x => x.Event)
            .Include(x => x.VenueSection)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public override Task<EventSection?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return Query()
            .Include(x => x.Event)
            .Include(x => x.VenueSection)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<EventSection>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken ct = default)
    {
        return await Query()
            .AsNoTracking()
            .Include(x => x.VenueSection)
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public Task<bool> ExistsByEventAndVenueSectionAsync(
        Guid eventId,
        Guid venueSectionId,
        CancellationToken ct = default)
    {
        return Query().AnyAsync(x =>
            x.EventId == eventId &&
            x.VenueSectionId == venueSectionId,
            ct);
    }
}