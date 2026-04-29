namespace Eventix.Application.DTOs.EventSessions;

public class CreateEventSessionDTO
{
    public Guid EventId { get; set; }
    public Guid? SpeakerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}