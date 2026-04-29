namespace Eventix.Application.DTOs.Events;

using Eventix.Domain.Enums;

public class EventResponseDTO
{
    public Guid Id { get; set; }
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

    public EventStatus Status { get; set; }
    public EventVisibility Visibility { get; set; }

    public string? BannerImageUrl { get; set; }

    public int MaxTicketsPerOrder { get; set; }
    public int MinTicketsPerOrder { get; set; }

    public bool IsFree { get; set; }
    public bool IsPublished { get; set; }

    public string Currency { get; set; } = "EUR";

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}