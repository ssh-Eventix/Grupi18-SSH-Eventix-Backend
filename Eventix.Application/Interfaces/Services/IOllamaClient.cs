namespace Eventix.Application.Interfaces.Services;

public interface IOllamaClient
{
    Task<string> GenerateAsync(string prompt, CancellationToken ct);
}