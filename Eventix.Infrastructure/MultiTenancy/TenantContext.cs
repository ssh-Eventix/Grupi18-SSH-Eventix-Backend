using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventix.Application.Interfaces.Common;

namespace Eventix.Infrastructure.MultiTenancy
{
    public class TenantContext : ITenantContext
    {
        public Guid? TenantId { get; set; }
        public string? SchemaName { get; set; }
    }
}
