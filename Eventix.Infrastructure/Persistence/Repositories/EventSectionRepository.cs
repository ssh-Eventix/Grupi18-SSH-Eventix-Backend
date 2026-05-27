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
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public override Task<EventSection?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return Query()
            .Include(x => x.Event)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<EventSection>> GetByEventIdAsync(
    Guid eventId,
    CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .Include(x => x.Event)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
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