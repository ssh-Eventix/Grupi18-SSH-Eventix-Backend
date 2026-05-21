using System.Net;
using System.Net.Http.Json;

namespace Eventix.ApiTests.Auth;

public class LoginApiTests
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://localhost:5225")
    };

    [Fact]
    public async Task Login_With_Invalid_Credentials_Should_Return_Unauthorized()
    {
        var response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    email = "wrong@test.com",
                    password = "WrongPassword123!"
                });

        Assert.True(
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.BadRequest);
    }
}