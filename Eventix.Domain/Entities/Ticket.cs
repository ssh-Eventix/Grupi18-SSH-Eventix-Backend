using Eventix.Domain.Common;
using Eventix.Domain.Enums;

namespace Eventix.Domain.Entities
{
    public class Ticket : TenantBaseEntity
    {
        public Guid BookingItemId { get; set; }
        public BookingItem BookingItem { get; set; } = default!;
        public string TicketCode { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Active;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UsedAt { get; set; }
        public CheckIn? CheckIn { get; set; }
    }
}
