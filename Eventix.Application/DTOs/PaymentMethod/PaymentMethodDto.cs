using System;

namespace Eventix.Application.DTOs.PaymentMethod
{
    public class PaymentMethodDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string Provider { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}