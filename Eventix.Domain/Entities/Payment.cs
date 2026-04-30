using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class Payment : TenantBaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public Guid PaymentMethodId { get; set; }

        [ForeignKey(nameof(PaymentMethodId))]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(200)]
        public string TransactionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed

        public DateTime? PaidAt { get; set; }
    }
}