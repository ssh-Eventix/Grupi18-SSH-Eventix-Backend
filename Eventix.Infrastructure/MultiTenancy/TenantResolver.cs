using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.MultiTenancy
{
    public class TenantResolver : ITenantResolver
    {
        private readonly PublicDbContext _context;

        public TenantResolver(PublicDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant?> ResolveAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive, cancellationToken);
        }
    }
}
