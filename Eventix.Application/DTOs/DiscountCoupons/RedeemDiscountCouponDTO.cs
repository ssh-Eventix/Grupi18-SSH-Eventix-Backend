namespace Eventix.Application.DTOs.DiscountCoupons;

public class RedeemDiscountCouponDTO
{
    public Guid EventId { get; set; }
    public string Code { get; set; } = string.Empty;
}
