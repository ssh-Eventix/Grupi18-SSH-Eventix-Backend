namespace Eventix.Application.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<(string Token, DateTime ExpiresAtUtc)> CreateAsync(Guid userId, CancellationToken ct);
    string Hash(string token);
}