using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.BackgroundJobs;

public class EventStatusUpdateJob
{
    private readonly TenantDbContext _context;

    public EventStatusUpdateJob(TenantDbContext context)
    {
        _context = context;
    }

    public async Task UpdateEventStatuses()
    {
        var now = DateTime.UtcNow;

        var events = await _context.Events
             .Where(x =>
                 x.Status == EventStatus.Published &&
                 x.EndUtc < now)
             .ToListAsync();

        foreach (var ev in events)
        {
            ev.Status = EventStatus.Completed;
        }

        await _context.SaveChangesAsync();
    }
}