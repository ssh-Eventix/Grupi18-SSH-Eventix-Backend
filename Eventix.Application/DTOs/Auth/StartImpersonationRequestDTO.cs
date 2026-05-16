namespace Eventix.Application.DTOs.Auth;

public class StartImpersonationRequestDTO
{
    public Guid TargetTenantId { get; set; }
    public Guid TargetPublicUserId { get; set; }
    public int Minutes { get; set; } = 10;
    public string? Reason { get; set; }
}