using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Eventix.Infrastructure.MultiTenancy;

public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        if (context is TenantDbContext tenantDbContext)
        {
            return (context.GetType(), tenantDbContext.CurrentSchema, designTime);
        }

        return (context.GetType(), designTime);
    }
}