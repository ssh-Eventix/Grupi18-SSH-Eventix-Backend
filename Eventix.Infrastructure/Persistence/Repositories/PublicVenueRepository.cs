using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class PublicVenueRepository : IPublicVenueRepository
{
    private readonly PublicDbContext _context;

    public PublicVenueRepository(PublicDbContext context)
    {
        _context = context;
    }

    public Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Venues
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }
}
