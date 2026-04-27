namespace Eventix.Application.DTOs.VenueSections;

using Eventix.Domain.Enums;

public class UpdateVenueSectionDTO
{
    public Guid VenueId { get; set; }

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public int Capacity { get; set; }
    public SeatType SeatType { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    public decimal? DefaultBasePrice { get; set; }
}