namespace Eventix.Application.DTOs.Events;

public class CreateEventDTO
{
    public Guid VenueId { get; set; }
    public Guid EventCategoryId { get; set; }

    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }

    public string? OrganizerName { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public int Visibility { get; set; } = 2;

    public string? BannerImageUrl { get; set; }

    public int MaxTicketsPerOrder { get; set; } = 10;
    public int MinTicketsPerOrder { get; set; } = 1;

    public bool IsFree { get; set; } = false;

    public string Currency { get; set; } = "EUR";
}