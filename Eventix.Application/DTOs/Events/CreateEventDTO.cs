namespace Eventix.Application.DTOs.Events;

public class CreateEventDTO
{
    public Guid VenueId { get; set; }

    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }

    public string? OrganizerName { get; set; }
    public string? OrganizerEmail { get; set; }
    public string? OrganizerPhone { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public DateTime? DoorsOpenUtc { get; set; }
    public DateTime? SalesStartUtc { get; set; }
    public DateTime? SalesEndUtc { get; set; }

    public int Visibility { get; set; } = 2;
    public string? BannerImageUrl { get; set; }
    public string? ThumbnailImageUrl { get; set; }

    public Guid EventCategoryId { get; set; }
    public string? Tags { get; set; }

    public int MaxTicketsPerOrder { get; set; } = 10;
    public int MinTicketsPerOrder { get; set; } = 1;

    public bool IsFree { get; set; } = false;
    public bool AllowWaitlist { get; set; } = false;
    public bool RequiresApproval { get; set; } = false;

    public string? TermsAndConditions { get; set; }
    public string? RefundPolicy { get; set; }

    public string Currency { get; set; } = "EUR";
}