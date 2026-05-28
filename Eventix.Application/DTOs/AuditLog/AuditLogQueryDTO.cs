using Eventix.Domain.Enums;

namespace Eventix.Application.DTOs.AuditLog;

public class AuditLogQueryDTO
{
    public Guid? UserId { get; set; }

    public string? EntityName { get; set; }

    public Guid? EntityId { get; set; }

    public AuditAction? Action { get; set; }

    public string? Search { get; set; }

    public DateTime? FromDateUtc { get; set; }

    public DateTime? ToDateUtc { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}