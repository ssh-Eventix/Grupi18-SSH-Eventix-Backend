using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
	public class PaymentMethod : TenantBaseEntity
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; }  // e.g. Card, PayPal, Cash

		public bool IsActive { get; set; } = true;

		[MaxLength(100)]
		public string Provider { get; set; }  // e.g. Stripe, PayPal

		[MaxLength(300)]
		public string Description { get; set; }

		// Navigation (optional but good practice)
		public ICollection<Payment> Payments { get; set; }
	}
}