namespace Eventix.Application.DTOs.Ai;

public class AiRequestLogDTO
{
    public Guid Id { get; set; }

    public string Prompt { get; set; } = string.Empty;

    public string? ResponseSummary { get; set; }

    public string RequestType { get; set; } = string.Empty;

    public int TokensUsed { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}