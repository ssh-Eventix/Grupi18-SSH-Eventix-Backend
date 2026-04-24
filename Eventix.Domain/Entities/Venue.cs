using Eventix.Domain.Common;
using Eventix.Domain.Enums;

namespace Eventix.Domain.Entities;

public class Venue : TenantBaseEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public string? Description { get; set; }

    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = default!;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = default!;

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public int TotalCapacity { get; set; }

    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public VenueStatus Status { get; set; } = VenueStatus.Active;

    public bool HasSeatingMap { get; set; } = false;
    public string? SeatingMapImageUrl { get; set; }

    public bool IsIndoor { get; set; } = true;
    public bool IsAccessible { get; set; } = true;
    public ICollection<VenueSection> Sections { get; set; } = new List<VenueSection>();
    public ICollection<Event> Events { get; set; } = new List<Event>();
}