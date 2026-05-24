using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class BookingItem : TenantBaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid TicketTypeId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal Subtotal => Quantity * UnitPrice;

        public Booking Booking { get; set; } = default!;
        public TicketType TicketType { get; set; } = default!;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public Guid? EventSectionId { get; set; }
        public EventSection? EventSection { get; set; }
    }
}
