namespace Eventix.Application.DTOs.Auth;

public class ImpersonationStartResult
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime AccessTokenExpiresAtUtc { get; set; }

    public Guid ImpersonationSessionId { get; set; }

    public bool IsImpersonating { get; set; } = true;

    public ImpersonationStartResult()
    {
    }

    public ImpersonationStartResult(
        string accessToken,
        DateTime accessTokenExpiresAtUtc,
        Guid impersonationSessionId)
    {
        AccessToken = accessToken;
        AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc;
        ImpersonationSessionId = impersonationSessionId;
    }
}