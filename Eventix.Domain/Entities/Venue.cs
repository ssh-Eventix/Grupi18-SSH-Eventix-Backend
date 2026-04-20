using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class Venue : TenantBaseEntity
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
    public int Capacity { get; set; }

    public ICollection<VenueSection> Sections { get; set; } = new List<VenueSection>();
    public ICollection<Event> Events { get; set; } = new List<Event>();
}