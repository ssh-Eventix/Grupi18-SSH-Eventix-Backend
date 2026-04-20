using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IVenueRepository
{
    Task<List<Venue>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Venue entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Venue entity);
    Task DeleteAsync(Venue entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}