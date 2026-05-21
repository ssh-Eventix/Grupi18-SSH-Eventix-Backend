using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Eventix.Infrastructure.Auth;
using Eventix.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Eventix.UnitTests.Authorization;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _service;

    public JwtTokenServiceTests()
    {
        var settings = new JwtSettings
        {
            SecretKey = "this-is-a-very-long-secret-key-for-tests-123456",
            Issuer = "Eventix.Tests",
            Audience = "Eventix.Tests",
            ExpirationMinutes = 60
        };

        _service = new JwtTokenService(
            Options.Create(settings));
    }

    [Fact]
    public async Task GenerateTokenAsync_Should_Return_Valid_Token()
    {
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var result = await _service.GenerateTokenAsync(
            subjectId: userId,
            email: "admin@test.com",
            tenantId: tenantId,
            roles: new[] { "Admin" });

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.True(result.ExpiresAtUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task Generated_Token_Should_Contain_Role_And_TenantId()
    {
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var result = await _service.GenerateTokenAsync(
            subjectId: userId,
            email: "admin@test.com",
            tenantId: tenantId,
            roles: new[] { "Admin" });

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        Assert.Contains(token.Claims, x =>
            x.Type == "tenantId" &&
            x.Value == tenantId.ToString());

        Assert.Contains(token.Claims, x =>
            x.Type == ClaimTypes.Role &&
            x.Value == "Admin");
    }

    [Fact]
    public async Task GenerateTokenAsync_For_SuperAdmin_Should_Add_SuperAdmin_Claim()
    {
        var result = await _service.GenerateTokenAsync(
            subjectId: Guid.NewGuid(),
            email: "superadmin@test.com",
            tenantId: Guid.Empty,
            roles: new[] { "SuperAdmin" },
            isSuperAdmin: true);

        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(result.Token);

        Assert.Contains(token.Claims, x =>
            x.Type == "isSuperAdmin" &&
            x.Value == "true");
    }

    [Fact]
    public async Task GenerateTokenAsync_For_Impersonation_Should_Add_Impersonation_Claims()
    {
        var sessionId = Guid.NewGuid();
        var impersonatorId = Guid.NewGuid();

        var result = await _service.GenerateTokenAsync(
            subjectId: Guid.NewGuid(),
            email: "admin@test.com",
            tenantId: Guid.NewGuid(),
            roles: new[] { "Admin" },
            impersonationSessionId: sessionId,
            impersonatorPublicUserId: impersonatorId,
            isImpersonation: true);

        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(result.Token);

        Assert.Contains(token.Claims, x =>
            x.Type == "isImpersonation" &&
            x.Value == "true");

        Assert.Contains(token.Claims, x =>
            x.Type == "impersonationSessionId" &&
            x.Value == sessionId.ToString());
    }
}