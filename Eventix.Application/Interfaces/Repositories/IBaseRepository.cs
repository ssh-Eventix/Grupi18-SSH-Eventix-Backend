namespace Eventix.Application.Interfaces.Repositories;

public interface IBaseRepository<TEntity>
{
    Task<List<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task SaveChangesAsync(CancellationToken ct = default);
}