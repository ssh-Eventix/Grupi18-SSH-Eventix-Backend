using System;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public enum ImpersonationEventType
    {
        Started = 0,
        Revoked = 1
    }
    public class TenantImpersonationEvent : BaseEntity
    {
        public Guid SessionId { get; set; }
        public ImpersonationEventType EventType { get; set; }
        public Guid? ActorPublicUserId { get; set; }
        public Guid? ActorTenantUserId { get; set; }
        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }
    }
}

