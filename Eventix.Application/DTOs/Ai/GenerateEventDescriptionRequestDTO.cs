namespace Eventix.Application.DTOs.Ai;

public class GenerateEventDescriptionRequestDTO
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}