using Eventix.Domain.Common;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;

public class VenueSection : TenantBaseEntity
{
    public Guid VenueId { get; set; }
    public Venue Venue { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public int Capacity { get; set; }
    public SeatType SeatType { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public decimal? DefaultBasePrice { get; set; }
    public ICollection<EventSection> EventSections { get; set; } = new List<EventSection>();
}