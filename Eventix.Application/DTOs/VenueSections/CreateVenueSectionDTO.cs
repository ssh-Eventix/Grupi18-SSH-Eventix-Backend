namespace Eventix.Application.DTOs.VenueSections;

public class CreateVenueSectionDTO
{
    public Guid VenueId { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public int Capacity { get; set; }
    public int SeatType { get; set; }
    public decimal? DefaultBasePrice { get; set; }
}