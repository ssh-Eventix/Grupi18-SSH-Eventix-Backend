using Eventix.Domain.Common;
using Eventix.Domain.Entities;

namespace Eventix.Domain.Entities;
public class EventSection : TenantBaseEntity
{
    public Guid EventId { get; set; }
    public Guid VenueSectionId { get; set; }

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime? SalesStartUtc { get; set; }
    public DateTime? SalesEndUtc { get; set; }

    public Event Event { get; set; } = default!;
    public VenueSection VenueSection { get; set; } = default!;
    public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
    public ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
}