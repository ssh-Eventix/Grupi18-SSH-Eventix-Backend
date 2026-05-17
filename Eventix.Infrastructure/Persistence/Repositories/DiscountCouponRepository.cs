using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class DiscountCouponRepository 
    : TenantBaseRepository<DiscountCoupon>, IDiscountCouponRepository
{
    public DiscountCouponRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }

    public Task<List<DiscountCoupon>> GetByEventIdAsync(Guid eventId, CancellationToken ct = default)
    {
        return Query()
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .ToListAsync(ct);
    }

    public Task<bool> ExistsByEventAndCodeAsync(
        Guid eventId,
        string code,
        CancellationToken ct = default)
    {
        var normalizedCode = code.ToLower();

        return Query().AnyAsync(x =>
            x.EventId == eventId &&
            x.Code.ToLower() == normalizedCode,
            ct);
    }
}