using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IVenueRepository
{
    Task<List<Venue>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Venue venue, CancellationToken cancellationToken = default);

    Task UpdateAsync(Venue venue);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}