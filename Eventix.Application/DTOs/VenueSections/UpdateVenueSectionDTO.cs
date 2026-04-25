namespace Eventix.Application.DTOs.VenueSections;

public class UpdateVenueSectionDTO
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public int Capacity { get; set; }
    public int SeatType { get; set; }
    public bool IsActive { get; set; }
    public decimal? DefaultBasePrice { get; set; }
}