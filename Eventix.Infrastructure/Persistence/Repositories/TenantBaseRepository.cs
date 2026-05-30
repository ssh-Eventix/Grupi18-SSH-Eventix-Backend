using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Common;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class TenantBaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : TenantBaseEntity
{
    protected readonly TenantDbContext Context;
    protected readonly ITenantContext TenantContext;
    protected readonly DbSet<TEntity> DbSet;

    protected Guid TenantId => TenantContext.TenantId;

    public TenantBaseRepository(
        TenantDbContext context,
        ITenantContext tenantContext)
    {
        Context = context;
        TenantContext = tenantContext;
        DbSet = context.Set<TEntity>();
    }

    protected virtual IQueryable<TEntity> Query()
    {
        return DbSet.Where(x =>
            x.TenantId == TenantId &&
            !x.IsDeleted);
    }

    public virtual Task<List<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return Query()
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public virtual Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return Query()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        entity.TenantId = TenantId;
        await DbSet.AddAsync(entity, ct);
    }

    public virtual Task UpdateAsync(TEntity entity)
    {
        entity.UpdatedAtUtc = DateTime.UtcNow;
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(TEntity entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public virtual Task SaveChangesAsync(CancellationToken ct = default)
    {
        return Context.SaveChangesAsync(ct);
    }
}