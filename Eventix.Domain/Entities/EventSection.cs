using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class EventSection : TenantBaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = default!;

    public Guid VenueSectionId { get; set; }
    public VenueSection VenueSection { get; set; } = default!;

    public string Name { get; set; } = default!; 
    public string Code { get; set; } = default!;

    public int Capacity { get; set; }
    public int ReservedSeats { get; set; } = 0;
    public int SoldSeats { get; set; } = 0;

    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";

    public bool IsActive { get; set; } = true;
    public bool IsHidden { get; set; } = false;
    public bool SalesEnabled { get; set; } = true;

    public DateTime? SalesStartUtc { get; set; }
    public DateTime? SalesEndUtc { get; set; }

    public int MaxTicketsPerOrder { get; set; } = 10;
    public int MinTicketsPerOrder { get; set; } = 1;

    public string? Benefits { get; set; } 
    public string? Notes { get; set; }
}