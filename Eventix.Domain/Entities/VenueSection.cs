using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class VenueSection : TenantBaseEntity
{
    public Guid VenueId { get; set; }
    public string Name { get; set; } = default!;
    public int Capacity { get; set; }
    public string? Description { get; set; }

    public Venue Venue { get; set; } = default!;
}