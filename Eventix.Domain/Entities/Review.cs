using Eventix.Domain.Common;

namespace Eventix.Domain.Entities
{
    public class Review : TenantBaseEntity
    {
        public Guid EventId { get; set; }

        public Event Event { get; set; } = default!;

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
