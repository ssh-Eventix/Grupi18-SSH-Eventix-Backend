using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IDiscountCouponRepository
{
    Task<List<DiscountCoupon>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DiscountCoupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<DiscountCoupon>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task AddAsync(DiscountCoupon entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(DiscountCoupon entity);
    Task DeleteAsync(DiscountCoupon entity);
    Task<bool> ExistsByEventAndCodeAsync(Guid eventId, string code, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
