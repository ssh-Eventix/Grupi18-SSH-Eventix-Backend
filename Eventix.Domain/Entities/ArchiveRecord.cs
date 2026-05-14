using Eventix.Domain.Common;

namespace Eventix.Domain.Entities;

public class ArchiveRecord : BaseEntity
{
    public Guid TenantId { get; set; }

    public string SchemaName { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public Guid EntityId { get; set; }

    public int ArchiveYear { get; set; }

    public string DataJson { get; set; } = null!;

    public DateTime ArchivedAtUtc { get; set; } = DateTime.UtcNow;
}