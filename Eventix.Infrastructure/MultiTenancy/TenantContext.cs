using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Infrastructure.MultiTenancy
{
    public class TenantContext : ITenantContext
    {
        public Guid TenantId { get; private set; }
        public string Schema { get; private set; } = "public";
        public string? TenantSlug { get; private set; }
        public void SetTenant(Guid tenantId, string tenantSlug, string schema)
        {
            TenantId = tenantId;
            TenantSlug = tenantSlug;
            Schema = schema;
        }
    }
}
