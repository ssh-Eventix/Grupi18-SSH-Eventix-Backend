using System;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{

    public class UserRole : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}