using System.Net;

namespace Eventix.ApiTests.Authorization;

public class PermissionApiTests
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://localhost:5225")
    };

    [Fact]
    public async Task Protected_Endpoint_Without_Token_Should_Return_Unauthorized()
    {
        _client.DefaultRequestHeaders.Add(
            "X-Tenant-Slug",
            "alpha-events");

        var response = await _client.GetAsync("/api/users");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}