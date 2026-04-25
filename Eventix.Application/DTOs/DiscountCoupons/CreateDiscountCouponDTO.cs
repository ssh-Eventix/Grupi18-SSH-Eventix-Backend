using System;
using Eventix.Domain.Enums;

namespace Eventix.Application.DTOs.DiscountCoupons;

public class CreateDiscountCouponDTO
{
    public Guid EventId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int? UsageLimit { get; set; }
}