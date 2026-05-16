using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class RoleRepository : TenantBaseRepository<Role>, IRoleRepository
{
    public RoleRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }
}