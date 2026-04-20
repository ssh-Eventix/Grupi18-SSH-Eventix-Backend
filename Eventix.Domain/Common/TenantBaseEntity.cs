using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Domain.Common
{
    public abstract class TenantBaseEntity : BaseEntity
    {
        public Guid TenantId { get; set; }
    }
}
