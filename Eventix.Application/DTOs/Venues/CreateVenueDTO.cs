namespace Eventix.Application.DTOs.Venues;

public class CreateVenueDTO
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string AddressLine1 { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
    public int TotalCapacity { get; set; }
}