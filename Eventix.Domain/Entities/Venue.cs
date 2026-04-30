using Eventix.Domain.Common;
using Eventix.Domain.Enums;

namespace Eventix.Domain.Entities;

public class Venue : TenantBaseEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public string AddressLine1 { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;

    public int TotalCapacity { get; set; }

    public bool IsIndoor { get; set; } = true;
    public bool IsAccessible { get; set; } = true;

    public ICollection<Event> Events { get; set; } = new List<Event>();
    public ICollection<VenueSection> Sections { get; set; } = new List<VenueSection>();
}