namespace Eventix.Application.DTOs.Ai;

public class GenerateEventDescriptionRequestDTO
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? OrganizerName { get; set; }
    public string? StartUtc { get; set; }
    public string? EndUtc { get; set; }
    public string? Currency { get; set; }
    public bool IsFree { get; set; }
}
