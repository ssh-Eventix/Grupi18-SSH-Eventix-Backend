using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct);

    Task<List<RefreshToken>> GetAllAsync(CancellationToken ct);

    Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken ct);

    Task UpdateAsync(RefreshToken token, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);

    Task<List<RefreshToken>> GetByPublicUserIdAsync(
    Guid publicUserId,
    CancellationToken ct);
}