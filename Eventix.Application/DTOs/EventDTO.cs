namespace Eventix.Application.DTOs.Events;

public class EventDTO
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid OrganizerId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid VenueId { get; set; }

    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Status { get; set; } = default!;

    public string? CategoryName { get; set; }
    public string? VenueName { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}