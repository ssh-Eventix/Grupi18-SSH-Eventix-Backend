namespace Eventix.Application.DTOs.EventSessions;

public class EventSessionResponseDTO
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public Guid? SpeakerId { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}