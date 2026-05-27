using Eventix.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public Guid PublicUserId { get; set; }
        public Guid? TenantId { get; set; }

        public string Email { get; set; } = string.Empty;
        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? UsedAtUtc { get; set; }

        public bool IsUsed => UsedAtUtc.HasValue;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

        public PublicUser PublicUser { get; set; } = default!;
    }
}
