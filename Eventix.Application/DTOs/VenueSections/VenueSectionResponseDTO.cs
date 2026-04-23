namespace Eventix.Application.DTOs.VenueSections;

public class VenueSectionResponseDTO
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid VenueId { get; set; }

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }

    public int Capacity { get; set; }
    public int SeatType { get; set; }

    public int? RowCount { get; set; }
    public int? SeatsPerRow { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsAccessibleSection { get; set; }
    public bool IsActive { get; set; }

    public decimal? DefaultBasePrice { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}