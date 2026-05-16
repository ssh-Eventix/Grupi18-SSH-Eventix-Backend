using Eventix.Application.Interfaces.Common;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class EventSessionRepository
    : TenantBaseRepository<EventSession>, IEventSessionRepository
{
    public EventSessionRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }

    public Task<List<EventSession>> GetByEventIdAsync(Guid eventId, CancellationToken ct = default)
    {
        return Query()
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .ToListAsync(ct);
    }
}