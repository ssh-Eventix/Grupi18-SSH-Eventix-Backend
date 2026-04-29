using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventix.Domain.Entities;

namespace Eventix.Infrastructure.MultiTenancy
{
    public interface ITenantResolver
    {
        Task<Tenant?> ResolveAsync(string slug, CancellationToken cancellationToken = default);
    }
}
