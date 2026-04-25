namespace Eventix.Application.DTOs.Events;

public class UpdateEventDTO
{
    public Guid VenueId { get; set; }
    public Guid EventCategoryId { get; set; }

    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }

    public string? OrganizerName { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public int Status { get; set; }
    public int Visibility { get; set; }

    public string? BannerImageUrl { get; set; }

    public int MaxTicketsPerOrder { get; set; }
    public int MinTicketsPerOrder { get; set; }

    public bool IsFree { get; set; }
    public bool IsPublished { get; set; }

    public string Currency { get; set; } = "EUR";
}