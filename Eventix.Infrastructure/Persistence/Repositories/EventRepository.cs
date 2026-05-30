using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class EventRepository : TenantBaseRepository<Event>, IEventRepository
{
    public EventRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }

    public async Task<List<Event>> GetAllAsync(
    string? search = null,
    CancellationToken ct = default)
    {
        IQueryable<Event> query = Query()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();

            query = query.Where(x =>
                x.Title.ToLower().Contains(normalizedSearch) ||
                (x.Description != null &&
                 x.Description.ToLower().Contains(normalizedSearch)) ||
                (x.OrganizerName != null &&
                 x.OrganizerName.ToLower().Contains(normalizedSearch)) ||
                x.Slug.ToLower().Contains(normalizedSearch));
        }

        return await query
            .OrderByDescending(x => x.StartUtc)
            .ToListAsync(ct);
    }

    public override Task<Event?> GetByIdAsync(Guid id,
CancellationToken ct = default)
    {
        return Query()
            .Include(x => x.EventSections)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
