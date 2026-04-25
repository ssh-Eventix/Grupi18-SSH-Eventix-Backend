namespace Eventix.Application.DTOs.Events;

public class EventResponseDTO
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public Guid VenueId { get; set; }
    public string? VenueName { get; set; }

    public Guid EventCategoryId { get; set; }
    public string? EventCategoryName { get; set; }

    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }

    public string? OrganizerName { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public string Status { get; set; } = default!;
    public string Visibility { get; set; } = default!;

    public string? BannerImageUrl { get; set; }

    public int MaxTicketsPerOrder { get; set; }
    public int MinTicketsPerOrder { get; set; }

    public bool IsFree { get; set; }
    public bool IsPublished { get; set; }

    public string Currency { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }
}