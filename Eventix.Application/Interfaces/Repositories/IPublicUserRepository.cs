using Eventix.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface IPublicUserRepository
    {
        Task<PublicUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<PublicUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(PublicUser entity, CancellationToken cancellationToken = default);
    }
}

