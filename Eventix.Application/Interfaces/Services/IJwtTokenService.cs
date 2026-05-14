using System.Security.Claims;

namespace Eventix.Application.Interfaces.Services;

public interface IJwtTokenService
{
    Task<(string Token, DateTime ExpiresAtUtc)> GenerateTokenAsync(
        Guid subjectId,
        string email,
        Guid tenantId,
        IEnumerable<string> roles,
        bool isSuperAdmin = false,
        Guid? impersonationSessionId = null,
        Guid? impersonatorPublicUserId = null,
        bool isImpersonation = false,
        CancellationToken cancellationToken = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}