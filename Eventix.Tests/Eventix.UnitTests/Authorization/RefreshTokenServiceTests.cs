using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Services;

namespace Eventix.UnitTests.Authorization;

public class RefreshTokenServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Refresh_Token()
    {
        var fakeRepo = new FakeRefreshTokenRepository();

        var service = new RefreshTokenService(fakeRepo);

        var publicUserId = Guid.NewGuid();

        var result = await service.CreateAsync(
            publicUserId,
            CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.True(result.ExpiresAtUtc > DateTime.UtcNow);

        Assert.NotNull(fakeRepo.AddedToken);
        Assert.Equal(publicUserId, fakeRepo.AddedToken.PublicUserId);
        Assert.False(string.IsNullOrWhiteSpace(fakeRepo.AddedToken.TokenHash));
        Assert.Equal(1, fakeRepo.SaveChangesCallCount);
    }

    [Fact]
    public void Hash_Should_Return_Same_Value_For_Same_Token()
    {
        var fakeRepo = new FakeRefreshTokenRepository();

        var service = new RefreshTokenService(fakeRepo);

        var token = "test-refresh-token";

        var hash1 = service.Hash(token);
        var hash2 = service.Hash(token);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Hash_Should_Not_Return_Original_Token()
    {
        var fakeRepo = new FakeRefreshTokenRepository();

        var service = new RefreshTokenService(fakeRepo);

        var token = "test-refresh-token";

        var hash = service.Hash(token);

        Assert.NotEqual(token, hash);
    }

    private class FakeRefreshTokenRepository : IRefreshTokenRepository
    {
        public RefreshToken? AddedToken { get; private set; }

        public int SaveChangesCallCount { get; private set; }

        public Task AddAsync(
            RefreshToken refreshToken,
            CancellationToken cancellationToken = default)
        {
            AddedToken = refreshToken;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.CompletedTask;
        }

        public Task<RefreshToken?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<RefreshToken?>(null);
        }

        public Task<IReadOnlyList<RefreshToken>> GetActiveTokensForUserAsync(
            Guid publicUserId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<RefreshToken>>(
                new List<RefreshToken>());
        }

        public void Update(RefreshToken refreshToken)
        {
        }

        public Task<List<RefreshToken>> GetAllAsync(CancellationToken ct)
        {
            return Task.FromResult(new List<RefreshToken>());
        }

        public Task UpdateAsync(RefreshToken token, CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public Task<List<RefreshToken>> GetByPublicUserIdAsync(Guid publicUserId, CancellationToken ct)
        {
            return Task.FromResult(new List<RefreshToken>());
        }
    }
}