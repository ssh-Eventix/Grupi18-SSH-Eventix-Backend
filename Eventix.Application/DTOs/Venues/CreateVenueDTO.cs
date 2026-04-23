namespace Eventix.Application.DTOs.Venues;

public class CreateVenueDTO
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }

    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = default!;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = default!;

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public int TotalCapacity { get; set; }

    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public bool HasSeatingMap { get; set; }
    public string? SeatingMapImageUrl { get; set; }

    public bool IsIndoor { get; set; } = true;
    public bool IsAccessible { get; set; } = true;
}