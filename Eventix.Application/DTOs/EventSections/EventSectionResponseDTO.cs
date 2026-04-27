namespace Eventix.Application.DTOs.EventSections;

public class EventSectionResponseDTO
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public Guid EventId { get; set; }
    public string? EventTitle { get; set; }

    public Guid VenueSectionId { get; set; }
    public string? VenueSectionName { get; set; }

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public int Capacity { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    public DateTime? SalesStartUtc { get; set; }
    public DateTime? SalesEndUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}