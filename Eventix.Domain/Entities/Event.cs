using System;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class Event : TenantBaseEntity
{
    public Guid OrganizerId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid VenueId { get; set; }

    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Status { get; set; } = "Draft";

    public EventCategory Category { get; set; } = default!;
    public Venue Venue { get; set; } = default!;
}