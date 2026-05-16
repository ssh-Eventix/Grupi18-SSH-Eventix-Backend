using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class UserRepository : TenantBaseRepository<User>, IUserRepository
{
    public UserRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
        : base(context, tenantContext)
    {
    }

    public Task<User?> GetByPublicUserIdAsync(Guid publicUserId, CancellationToken ct)
    {
        return Query()
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.PublicUserId == publicUserId, ct);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalizedEmail = email.ToLower();

        return Query()
            .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail, ct);
    }

    public Task<User?> GetByEmailAndTenantAsync(string email, Guid tenantId, CancellationToken ct)
    {
        var normalizedEmail = email.ToLower();

        return DbSet.FirstOrDefaultAsync(x =>
            x.Email.ToLower() == normalizedEmail &&
            x.TenantId == tenantId &&
            !x.IsDeleted,
            ct);
    }
}