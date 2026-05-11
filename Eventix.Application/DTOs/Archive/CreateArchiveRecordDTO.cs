

namespace Eventix.Application.DTOs.Archive
{
    public class CreateArchiveRecordDTO
    {
        public Guid TenantId { get; set; }
        public string SchemaName { get; set; } = null!;
        public string EntityName { get; set; } = null!;
        public Guid EntityId { get; set; }
        public int ArchiveYear { get; set; }
        public string DataJson { get; set; } = null!;
    }
}
