namespace Eventix.Application.DTOs.Ai;

public class AiResponseDTO
{
    public string Response { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
}