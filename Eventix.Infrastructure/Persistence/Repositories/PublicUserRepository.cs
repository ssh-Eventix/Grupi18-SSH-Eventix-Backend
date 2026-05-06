using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eventix.Infrastructure.Persistence.Repositories
{
    public class PublicUserRepository : IPublicUserRepository
    {
        private readonly PublicDbContext _context;

        public PublicUserRepository(PublicDbContext context)
        {
            _context = context;
        }

        public Task<PublicUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => _context.PublicUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower(), cancellationToken);

        public Task<PublicUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.PublicUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task AddAsync(PublicUser entity, CancellationToken cancellationToken = default)
            => await _context.PublicUsers.AddAsync(entity, cancellationToken);
    }
}

