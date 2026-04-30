using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Eventix.Domain.Common;
using Eventix.Domain.Enums;

namespace Eventix.Domain.Entities
{
	public class PaymentMethod : TenantBaseEntity
	{
		public string Name { get; set; }  

		public bool IsActive { get; set; } = true;

        public PaymentProvider Provider { get; set; } 

        public string Description { get; set; }

		public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}