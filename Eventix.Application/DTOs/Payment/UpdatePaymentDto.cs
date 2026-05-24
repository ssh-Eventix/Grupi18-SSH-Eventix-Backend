using System;
using Eventix.Domain.Enums;

namespace Eventix.Application.DTOs.Payment
{
    public class UpdatePaymentDto
    {
        public Guid PaymentMethodId { get; set; }

        public string? TransactionId { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}