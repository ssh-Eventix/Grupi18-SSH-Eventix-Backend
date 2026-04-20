using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class EventCategory : TenantBaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<Event> Events { get; set; } = new List<Event>();
}