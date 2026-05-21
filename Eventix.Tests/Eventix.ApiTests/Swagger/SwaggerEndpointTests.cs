using System.Net;

namespace Eventix.ApiTests.Swagger;

public class SwaggerEndpointTests
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://localhost:5225")
    };

    [Fact]
    public async Task Swagger_Should_Be_Available()
    {
        var response = await _client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}