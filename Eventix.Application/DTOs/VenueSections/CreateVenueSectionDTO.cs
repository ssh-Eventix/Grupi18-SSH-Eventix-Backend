namespace Eventix.Application.DTOs.VenueSections;

using Eventix.Domain.Enums;

public class CreateVenueSectionDTO
{
    public Guid VenueId { get; set; }

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public int Capacity { get; set; }
    public SeatType SeatType { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public decimal? DefaultBasePrice { get; set; }
}