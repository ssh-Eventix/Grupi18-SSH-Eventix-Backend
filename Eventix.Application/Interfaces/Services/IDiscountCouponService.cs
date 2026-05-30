using Eventix.Application.DTOs.DiscountCoupons;

namespace Eventix.Application.Interfaces.Services;

public interface IDiscountCouponService
{
    Task<List<DiscountCouponResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DiscountCouponResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<DiscountCouponResponseDTO>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<DiscountCouponResponseDTO> CreateAsync(CreateDiscountCouponDTO dto, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateDiscountCouponDTO dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

