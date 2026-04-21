using Eventix.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public Guid BookingItemId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Active;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        //Navigation property
        public BookingItem BookingItem { get; set; }
    }

    public enum TicketStatus
    {
        Active,
        Used,
        Cancelled,
        Refunded
    }
}
