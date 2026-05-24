using Eventix.Domain.Enums;
using System;

namespace Eventix.Application.DTOs.Payment
{
    public class PaymentDto
    {
        public Guid Id { get; set; }

        public Guid BookingId { get; set; }

        public decimal Amount { get; set; }

        public Guid PaymentMethodId { get; set; }

        public String Status { get; set; } = String.Empty;

        public DateTime? PaidAt { get; set; }
    }
}