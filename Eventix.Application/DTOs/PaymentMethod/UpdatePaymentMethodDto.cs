using Eventix.Domain.Enums;

namespace Eventix.Application.DTOs.PaymentMethod
{
    public class UpdatePaymentMethodDto
    {
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public PaymentProvider Provider { get; set; }

        public string? Description { get; set; }
    }
}