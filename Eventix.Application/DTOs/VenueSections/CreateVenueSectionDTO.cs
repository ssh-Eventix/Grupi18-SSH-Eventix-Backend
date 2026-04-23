namespace Eventix.Application.DTOs.VenueSections;

public class CreateVenueSectionDTO
{
    public Guid VenueId { get; set; }

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }

    public int Capacity { get; set; }

    public int SeatType { get; set; }

    public int? RowCount { get; set; }
    public int? SeatsPerRow { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsAccessibleSection { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public decimal? DefaultBasePrice { get; set; }
}