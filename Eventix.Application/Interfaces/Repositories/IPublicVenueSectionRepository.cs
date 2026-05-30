using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IPublicVenueSectionRepository
{
    Task<VenueSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
