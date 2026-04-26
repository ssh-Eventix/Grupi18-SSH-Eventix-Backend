using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Application.Interfaces.Common;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class DiscountCouponRepository : IDiscountCouponRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DiscountCouponRepository(TenantDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public Task<List<DiscountCoupon>> GetAllAsync(CancellationToken cancellationToken = default)
        => _context.DiscountCoupons
            .AsNoTracking()
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<DiscountCoupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.DiscountCoupons
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenantContext.TenantId && !x.IsDeleted, cancellationToken);

    public Task<List<DiscountCoupon>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        => _context.DiscountCoupons
            .AsNoTracking()
            .Where(x => x.EventId == eventId && x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(DiscountCoupon entity, CancellationToken cancellationToken = default)
        => await _context.DiscountCoupons.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(DiscountCoupon entity)
    {
        _context.DiscountCoupons.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(DiscountCoupon entity)
    {
        _context.DiscountCoupons.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByEventAndCodeAsync(Guid eventId, string code, CancellationToken cancellationToken = default)
        => _context.DiscountCoupons
            .AnyAsync(x => x.EventId == eventId && x.TenantId == _tenantContext.TenantId && x.Code.ToLower() == code.ToLower() && !x.IsDeleted, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}

