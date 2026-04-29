using Eventix.Domain.Common;
using Eventix.Domain.Enums;

namespace Eventix.Domain.Entities;

public class Event : TenantBaseEntity
{
    public Guid VenueId { get; set; }
    public Venue Venue { get; set; } = default!;

    public Guid EventCategoryId { get; set; }
    public EventCategory EventCategory { get; set; } = default!;

    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }

    public string? OrganizerName { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public EventStatus Status { get; set; } = EventStatus.Draft;
    public EventVisibility Visibility { get; set; } = EventVisibility.Public;

    public string? BannerImageUrl { get; set; }

    public int MaxTicketsPerOrder { get; set; } = 10;
    public int MinTicketsPerOrder { get; set; } = 1;

    public bool IsFree { get; set; } = false;
    public bool IsPublished { get; set; } = false;

    public string Currency { get; set; } = "EUR";

    public ICollection<EventSection> EventSections { get; set; } = new List<EventSection>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
    public ICollection<EventSession> Sessions { get; set; } = new List<EventSession>();
}