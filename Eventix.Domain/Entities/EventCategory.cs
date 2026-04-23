using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class EventCategory : TenantBaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public ICollection<Event> Events { get; set; } = new List<Event>();
}