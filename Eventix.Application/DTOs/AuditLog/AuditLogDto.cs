using System;

namespace Eventix.Application.DTOs.AuditLog
{
    public class AuditLogDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string? UserEmail { get; set; }

        public string EntityName { get; set; } = string.Empty;

        public Guid EntityId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}