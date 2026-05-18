//using System.Net;
//using System.Net.Http.Json;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Xunit;

namespace Eventix.ApiTests;

//public class TenantsApiTests : IClassFixture<WebApplicationFactory<Program>>
//{
//    private readonly HttpClient _client;

//    public TenantsApiTests(WebApplicationFactory<Program> factory)
//    {
//        _client = factory.CreateClient();
//    }

//    [Fact]
//    public async Task CreateTenant_Without_Token_Should_Return_Unauthorized()
//    {
//        var payload = new
//        {
//            name = "Alpha Events",
//            slug = "alpha-events-test",
//            schemaName = "tenant_alpha_events_test",
//            description = "Test tenant",
//            contactEmail = "admin@alpha-events.test",
//            city = "Prishtina",
//            country = "Kosovo",
//            logoUrl = "https://cdn.test/alpha-events.png",
//            status = 1,
//            isTrial = true,
//            isActive = true
//        };

//        var response = await _client.PostAsJsonAsync("/api/tenants", payload);

//        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//    }
//}