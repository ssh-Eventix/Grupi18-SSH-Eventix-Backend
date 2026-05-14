using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eventix.Domain.Common;
using Eventix.Domain.Enums;

namespace Eventix.Domain.Entities
{
    public class AuditLog : TenantBaseEntity
    {
        public Guid UserId { get; set; } //foreign key that relates to Users Table
<<<<<<< Updated upstream
=======
    
>>>>>>> Stashed changes
        public User User { get; set; } = default!;

        public string EntityName { get; set; } = string.Empty;

        public Guid EntityId { get; set; }

        public AuditAction Action { get; set; }

        public string? OldValues { get; set; }  

        public string? NewValues { get; set; }  

    }
}