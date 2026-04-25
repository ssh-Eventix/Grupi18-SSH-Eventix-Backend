using Eventix.Domain.Common;

public class EventSection : TenantBaseEntity
{
    public Guid EventId { get; set; }
    public Guid VenueSectionId { get; set; }

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public int Capacity { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? SalesStartUtc { get; set; }
    public DateTime? SalesEndUtc { get; set; }
}