using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly TenantDbContext _context;

    public RefreshTokenRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        await _context.RefreshTokens.AddAsync(token, ct);
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct)
    {
        return _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
    }

    public Task UpdateAsync(RefreshToken token, CancellationToken ct)
    {
        _context.RefreshTokens.Update(token);
        return Task.CompletedTask;
    }
}