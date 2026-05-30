using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.DTOs.DiscountCoupons
{
    public class ValidateDiscountCouponDTO
    {
        public Guid EventId { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
    }
}
