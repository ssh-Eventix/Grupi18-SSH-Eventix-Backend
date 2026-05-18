using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly PublicDbContext _context;

    public RefreshTokenRepository(PublicDbContext context)
    {
        _context = context;
    }
    public Task<List<RefreshToken>> GetAllAsync(CancellationToken ct)
    {
        return _context.RefreshTokens
            .Include(x => x.PublicUser)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }
    public async Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        await _context.RefreshTokens.AddAsync(token, ct);
    }

    public Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken ct)
    {
        return _context.RefreshTokens
            .Include(x => x.PublicUser)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
    }

    public Task UpdateAsync(RefreshToken token, CancellationToken ct)
    {
        _context.RefreshTokens.Update(token);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }

    public Task<List<RefreshToken>> GetByPublicUserIdAsync(
    Guid publicUserId,
    CancellationToken ct)
    {
        return _context.RefreshTokens
            .Include(x => x.PublicUser)
            .Where(x => x.PublicUserId == publicUserId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }
}