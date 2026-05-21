using Eventix.Application.DTOs.CheckIns;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Services;
using Eventix.Domain.Entities;

namespace Eventix.UnitTests.Services;

public class CheckInServiceTests
{
    [Fact]
    public async Task CreateAsync_When_Ticket_Already_CheckedIn_Should_Throw()
    {
        var repo = new FakeCheckInRepository
        {
            ExistingCheckIn = new CheckIn()
        };

        var tenantContext = new FakeTenantContext
        {
            TenantId = Guid.NewGuid(),
            SchemaName = "tenant_test"
        };

        var service = new CheckInService(
            repo,
            tenantContext);

        var dto = new CreateCheckInDTO
        {
            TicketId = Guid.NewGuid(),
            CheckedInByUserId = Guid.NewGuid(),
            Notes = "Already checked"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_Should_Create_CheckIn()
    {
        var tenantId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var repo = new FakeCheckInRepository();

        var tenantContext = new FakeTenantContext
        {
            TenantId = tenantId,
            SchemaName = "tenant_test"
        };

        var service = new CheckInService(
            repo,
            tenantContext);

        var dto = new CreateCheckInDTO
        {
            TicketId = ticketId,
            CheckedInByUserId = userId,
            Notes = "Valid"
        };

        var result = await service.CreateAsync(
            dto,
            CancellationToken.None);

        Assert.Equal(ticketId, result.TicketId);
        Assert.Equal(userId, result.CheckedInByUserId);

        Assert.NotNull(repo.AddedCheckIn);
        Assert.Equal(tenantId, repo.AddedCheckIn.TenantId);
        Assert.Equal(ticketId, repo.AddedCheckIn.TicketId);
        Assert.Equal(1, repo.SaveChangesCallCount);
    }

    private class FakeTenantContext : ITenantContext
    {
        public Guid TenantId { get; set; }

        public string? SchemaName { get; set; }
    }

    private class FakeCheckInRepository : ICheckInRepository
    {
        public CheckIn? ExistingCheckIn { get; set; }

        public CheckIn? AddedCheckIn { get; private set; }

        public int SaveChangesCallCount { get; private set; }

        public Task<CheckIn?> GetByTicketIdAsync(
            Guid ticketId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ExistingCheckIn);
        }

        public Task AddAsync(
            CheckIn checkIn,
            CancellationToken cancellationToken = default)
        {
            AddedCheckIn = checkIn;
            return Task.CompletedTask;
        }

        public Task<CheckIn?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<CheckIn?>(null);
        }

        public Task<IReadOnlyList<CheckIn>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<CheckIn>>(
                new List<CheckIn>());
        }

        public void Update(CheckIn checkIn)
        {
        }

        public void Delete(CheckIn checkIn)
        {
        }

        public Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.CompletedTask;
        }

        Task<List<CheckIn>> ICheckInRepository.GetAllAsync(CancellationToken ct)
        {
            return Task.FromResult(new List<CheckIn>());
        }
    }
}