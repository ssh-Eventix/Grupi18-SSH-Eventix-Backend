using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class EventCategoryRepository
    : TenantBaseRepository<EventCategory>, IEventCategoryRepository
{
    public EventCategoryRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }

    public override Task<List<EventCategory>> GetAllAsync(CancellationToken ct = default)
    {
        return Query()
            .AsNoTracking()
            .Include(x => x.Events)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);
    }

    public override Task<EventCategory?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return Query()
            .Include(x => x.Events)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}