using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface IPublicUserRepository
    {
        Task<PublicUser?> GetByEmailAsync(string email, CancellationToken ct);
        Task<PublicUser?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(PublicUser user, CancellationToken ct);
        Task UpdateAsync(PublicUser user, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}

