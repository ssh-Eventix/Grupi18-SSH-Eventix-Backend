using Eventix.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventix.Domain.Entities;

namespace Eventix.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public string ReferenceNumber { get; set; } = string.Empty;

        // Navigation properties
        public User User { get; set; } = default!;
        public Event Event { get; set; } = default!;
        public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Refunded
    }
}
