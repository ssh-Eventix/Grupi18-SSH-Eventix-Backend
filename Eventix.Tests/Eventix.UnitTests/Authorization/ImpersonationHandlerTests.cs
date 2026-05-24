using System.Security.Claims;
using Eventix.Api.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Eventix.UnitTests.Authorization;

public class ImpersonationHandlerTests
{
    [Fact]
    public async Task SuperAdmin_Not_Impersonating_Should_Succeed()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Role, "SuperAdmin")
            ],
            "TestAuth"));

        var requirement =
            new SuperAdminImpersonationRequirement();

        var context =
            new AuthorizationHandlerContext(
                [requirement],
                user,
                null);

        var handler = new ImpersonationHandler();

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task SuperAdmin_Impersonating_Should_Fail()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Role, "SuperAdmin"),
                new Claim("isImpersonation", "true")
            ],
            "TestAuth"));

        var requirement =
            new SuperAdminImpersonationRequirement();

        var context =
            new AuthorizationHandlerContext(
                [requirement],
                user,
                null);

        var handler = new ImpersonationHandler();

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Admin_Should_Not_Impersonate()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Role, "Admin")
            ],
            "TestAuth"));

        var requirement =
            new SuperAdminImpersonationRequirement();

        var context =
            new AuthorizationHandlerContext(
                [requirement],
                user,
                null);

        var handler = new ImpersonationHandler();

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }
}