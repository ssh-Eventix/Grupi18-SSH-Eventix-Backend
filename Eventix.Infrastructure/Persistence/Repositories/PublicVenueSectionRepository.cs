using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class PublicVenueSectionRepository : IPublicVenueSectionRepository
{
    private readonly PublicDbContext _context;

    public PublicVenueSectionRepository(PublicDbContext context)
    {
        _context = context;
    }

    public Task<VenueSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.VenueSections
            .AsNoTracking()
            .Include(x => x.Venue)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }
}
