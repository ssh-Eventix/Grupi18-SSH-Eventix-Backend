using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class ReviewRepository : TenantBaseRepository<Review>, IReviewRepository
{
    public ReviewRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }
}