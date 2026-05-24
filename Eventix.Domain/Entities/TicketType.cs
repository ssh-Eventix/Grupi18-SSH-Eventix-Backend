using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class TicketType : TenantBaseEntity
    {
        public Guid EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
        public int SoldQuantity { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
        public Event Event { get; set; } = default!;
        public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
        public Guid EventSectionId { get; set; }
        public EventSection EventSection { get; set; } = default!;
    }
}
