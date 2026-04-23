namespace Eventix.Application.DTOs.Events;

public class UpdateEventDTO
{
    public Guid OrganizerId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid VenueId { get; set; }

    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Status { get; set; } = default!;
}