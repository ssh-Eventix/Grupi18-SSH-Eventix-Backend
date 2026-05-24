using System.Net.Http.Json;
using System.Text.Json;
using Eventix.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Eventix.Infrastructure.Services;

public class OllamaClient : IOllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly string _model;

    public OllamaClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _model = configuration["Ollama:Model"] ?? "llama3.2";
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken ct)
    {
        var request = new
        {
            model = _model,
            prompt,
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", request, ct);

        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Ollama request failed: {json}");

        using var document = JsonDocument.Parse(json);

        return document.RootElement
            .GetProperty("response")
            .GetString() ?? string.Empty;
    }
}
