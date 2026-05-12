using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.BackgroundJobs;

public class CouponExpirationJob
{
    private readonly TenantDbContext _context;

    public CouponExpirationJob(TenantDbContext context)
    {
        _context = context;
    }

    public async Task ExpireCoupons()
    {
        var now = DateTime.UtcNow;

        var coupons = await _context.DiscountCoupons
            .Where(x => x.ValidTo < now)
            .ToListAsync();

        foreach (var c in coupons)
        {
            c.UsageLimit = 0;
        }

        await _context.SaveChangesAsync();
    }
}