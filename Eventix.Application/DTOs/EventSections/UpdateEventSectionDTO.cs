namespace Eventix.Application.DTOs.EventSections;

public class UpdateEventSectionDTO
{
    public Guid EventId { get; set; }
    public Guid VenueSectionId { get; set; }

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public int Capacity { get; set; }

    public bool IsActive { get; set; }

    public DateTime? SalesStartUtc { get; set; }
    public DateTime? SalesEndUtc { get; set; }
}