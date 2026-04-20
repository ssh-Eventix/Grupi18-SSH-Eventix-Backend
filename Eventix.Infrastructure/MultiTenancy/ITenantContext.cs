using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Infrastructure.MultiTenancy
{
    public interface ITenantContext
    {
        Guid? TenantId { get; }
        string Schema { get; }
        string? TenantSlug { get; }
        void SetTenant(Guid tenantId, string tenantSlug, string schema);
    }
}
