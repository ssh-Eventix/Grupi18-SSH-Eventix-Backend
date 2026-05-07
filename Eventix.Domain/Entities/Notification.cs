using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventix.Domain.Common;
using Eventix.Domain.Enums;

namespace Eventix.Domain.Entities
{
    public class Notification : TenantBaseEntity
    {
        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public Guid? EventId { get; set; }

        public Event? Event { get; set; }

        public NotificationType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
