using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IPublicVenueRepository
{
    Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
