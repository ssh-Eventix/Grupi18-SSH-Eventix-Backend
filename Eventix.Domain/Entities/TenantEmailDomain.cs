using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class TenantEmailDomain : BaseEntity
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public string Domain { get; set; } = string.Empty;

    public string DefaultRoleName { get; set; } = "Buyer";

    public bool AutoApprove { get; set; } = false;
}