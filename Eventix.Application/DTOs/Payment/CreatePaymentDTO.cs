using System;
using Eventix.Domain.Enums;

namespace Eventix.Application.DTOs.Payment
{
    public class CreatePaymentDto
    {
        public Guid PaymentMethodId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime? PaidAt { get; set; }

        public Guid BookingId { get; set; }
    }
}