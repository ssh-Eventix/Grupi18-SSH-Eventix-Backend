using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Application.Interfaces.Common;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenantContext;

    public UserRoleRepository(TenantDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public Task<UserRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.UserRoles
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public Task<List<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => _context.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(UserRole entity, CancellationToken cancellationToken = default)
        => await _context.UserRoles.AddAsync(entity, cancellationToken);

    public Task DeleteAsync(UserRole entity)
    {
        _context.UserRoles.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        => _context.UserRoles
            .AnyAsync(x => x.UserId == userId && x.RoleId == roleId && !x.IsDeleted, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}

