using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eventix.Domain.Common;
using Eventix.Domain.Enums;

namespace Eventix.Domain.Entities
{
    public class AIRequestLog : TenantBaseEntity
    {
        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public string Prompt { get; set; }

        public string? ResponseSummary { get; set; }

        public AIRequestType RequestType { get; set; }

        public int TokensUsed { get; set; }
        public AIRequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}