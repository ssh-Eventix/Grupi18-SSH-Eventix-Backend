using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Application.Interfaces.Common;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenantContext;

    public RoleRepository(TenantDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        => _context.Roles
            .AsNoTracking()
            .Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Roles
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == _tenantContext.TenantId && !x.IsDeleted, cancellationToken);

    public async Task AddAsync(Role entity, CancellationToken cancellationToken = default)
        => await _context.Roles.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Role entity)
    {
        _context.Roles.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Role entity)
    {
        _context.Roles.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}

