namespace Eventix.Application.DTOs.User;

public class LoginRequestDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    // Optional: provide tenant slug for multi-tenant login (AuthController will resolve to tenant id)
    public string? TenantSlug { get; set; }
}