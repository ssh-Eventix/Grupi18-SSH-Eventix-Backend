using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Common
{
    public interface ITenantContext
    {
        Guid? TenantId { get; set; }
        string? SchemaName { get; set; }
    }
}
