using Eventix.Domain.Common;
using Eventix.Domain.Enums;

namespace Eventix.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid? TenantId { get; set; }

    public string? TenantName { get; set; }

    public Guid? UserId { get; set; }

    public string? UserEmail { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    public AuditAction Action { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }
}