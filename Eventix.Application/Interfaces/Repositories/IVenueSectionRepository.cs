using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IVenueSectionRepository
{
    Task<List<VenueSection>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<VenueSection>> GetByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default);

    Task<VenueSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(VenueSection entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(VenueSection entity);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}