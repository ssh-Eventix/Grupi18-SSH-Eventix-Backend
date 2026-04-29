using System;
using Eventix.Domain.Enums;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{

    public class DiscountCoupon : TenantBaseEntity
    {
        public Guid EventId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DiscountType Type { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; } = 0;

        public virtual Event Event { get; set; } = null!;
    }
}