namespace Eventix.Application.DTOs.Venues;

public class CreateVenueDTO
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
    public int Capacity { get; set; }
}