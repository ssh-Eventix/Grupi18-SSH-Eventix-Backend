using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class AuditLog : TenantBaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; }  // e.g. "Payment", "Event"

        [Required]
        public Guid EntityId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; }  // Create, Update, Delete

        public string OldValues { get; set; }  // JSON (nullable)

        public string NewValues { get; set; }  // JSON (nullable)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}