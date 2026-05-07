using System;
using System.Collections.Generic;
using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{

    public class User : TenantBaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
        public ICollection<AIRequestLog> AIRequestLogs { get; set; } = new List<AIRequestLog>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}