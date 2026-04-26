using System;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class EventSession : TenantBaseEntity
    {
        public Guid EventId { get; set; }
        public Guid? SpeakerId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public virtual Event Event { get; set; } = null!;
        public virtual Speaker? Speaker { get; set; }
    }
}