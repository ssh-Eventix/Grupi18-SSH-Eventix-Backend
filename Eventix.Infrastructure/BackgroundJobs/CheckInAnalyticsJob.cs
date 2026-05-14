using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.BackgroundJobs;

public class CheckInAnalyticsJob
{
    private readonly TenantDbContext _context;

    public CheckInAnalyticsJob(TenantDbContext context)
    {
        _context = context;
    }

    public async Task GenerateStats()
    {
        var today = DateTime.UtcNow.Date;

        var checkIns = await _context.CheckIns
            .Where(x => x.CheckInTime.Date == today)
            .ToListAsync();

        var total = checkIns.Count;

        Console.WriteLine($"Check-ins today: {total}");
    }
}