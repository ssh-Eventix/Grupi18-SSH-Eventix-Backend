using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class AIRequestLog : TenantBaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        // TenantId already comes from TenantBaseEntity

        [Required]
        public string Prompt { get; set; }

        [MaxLength(500)]
        public string ResponseSummary { get; set; }

        [MaxLength(100)]
        public string RequestType { get; set; } // Chat, Recommendation, Analysis

        public int TokensUsed { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } // Success, Failed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}