using Eventix.Application.Interfaces.Common;

namespace Eventix.Infrastructure.MultiTenancy
{
    public class TenantContext : ITenantContext
    {
        public Guid TenantId { get; set; }
        public string? SchemaName { get; set; }
    }
}
