namespace Eventix.Application.DTOs.Auth;

public class LoginResponseDTO
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAtUtc { get; set; }

    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAtUtc { get; set; }

    public bool IsImpersonation { get; set; }
    public Guid? ImpersonationSessionId { get; set; }
}