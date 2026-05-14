using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class PublicUserRepository : IPublicUserRepository
{
    private readonly PublicDbContext _context;

    public PublicUserRepository(PublicDbContext context)
    {
        _context = context;
    }

    public async Task<PublicUser?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _context.PublicUsers
            .Include(x => x.PublicUserRoles)
            .ThenInclude(x => x.PublicRole)
            .FirstOrDefaultAsync(x => x.Email == email, ct);
    }

    public async Task<PublicUser?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.PublicUsers
            .Include(x => x.PublicUserRoles)
            .ThenInclude(x => x.PublicRole)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(PublicUser user, CancellationToken ct)
    {
        await _context.PublicUsers.AddAsync(user, ct);
    }

    public Task UpdateAsync(PublicUser user, CancellationToken ct)
    {
        _context.PublicUsers.Update(user);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}