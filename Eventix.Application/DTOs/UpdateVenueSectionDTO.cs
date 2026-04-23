namespace Eventix.Application.DTOs.VenueSections;

public class UpdateVenueSectionDTO
{
    public Guid VenueId { get; set; }
    public string Name { get; set; } = default!;
    public int Capacity { get; set; }
    public string? Description { get; set; }
}