namespace Eventix.Application.DTOs.TenantEmailDomains;

public class TenantEmailDomainResponseDTO
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Domain { get; set; } = string.Empty;
    public string DefaultRoleName { get; set; } = string.Empty;
    public bool AutoApprove { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}
