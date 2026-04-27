using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class Notification : TenantBaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public Guid? EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event Event { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
