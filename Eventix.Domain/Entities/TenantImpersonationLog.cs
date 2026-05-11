using Eventix.Domain.Common;
using Eventix.Domain.Enums;

namespace Eventix.Domain.Entities;

public class TenantImpersonationLog : BaseEntity
{
    public Guid SuperAdminUserId { get; set; }
    public PublicUser SuperAdminUser { get; set; } = null!;

    public Guid TargetTenantId { get; set; }
    public Tenant TargetTenant { get; set; } = null!;

    public Guid? TargetUserId { get; set; }
    public PublicUser? TargetUser { get; set; }

    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAtUtc { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Reason { get; set; }

    public TenantImpersonationEvent Event { get; set; }

    public DateTime? RevokedAtUtc { get; set; }
}