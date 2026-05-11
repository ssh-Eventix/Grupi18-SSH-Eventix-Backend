using System;

namespace Eventix.Application.DTOs.Payment
{
    public class PaymentDto
    {
        public Guid Id { get; set; }

        public Guid BookingId { get; set; }

        public decimal Amount { get; set; }

        public Guid PaymentMethodId { get; set; }

        public string? TransactionId { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? PaidAt { get; set; }
    }
}