using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class TenantImpersonationLog : BaseEntity
    { 
        public Guid TenantId { get; set; }
        public Guid? ImpersonatorPublicUserId { get; set; }
        public Guid? ImpersonatorTenantUserId { get; set; }
        public Guid TargetTenantUserId { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Reason { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
    }
}

