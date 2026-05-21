using Eventix.Application.DTOs.DiscountCoupons;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;

namespace Eventix.UnitTests.Services;

public class DiscountCouponServiceTests
{
    [Fact]
    public async Task CreateAsync_When_Code_Exists_Should_Throw()
    {
        var repo = new FakeDiscountCouponRepository
        {
            ExistsByEventAndCode = true
        };

        var tenantContext = new FakeTenantContext
        {
            TenantId = Guid.NewGuid(),
            SchemaName = "tenant_test"
        };

        var service = new DiscountCouponService(
            repo,
            tenantContext);

        var dto = new CreateDiscountCouponDTO
        {
            EventId = Guid.NewGuid(),
            Code = "SAVE10",
            Type = DiscountType.Percentage,
            DiscountValue = 10,
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddDays(5),
            UsageLimit = 100
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(dto, Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Coupon()
    {
        var tenantId = Guid.NewGuid();

        var repo = new FakeDiscountCouponRepository
        {
            ExistsByEventAndCode = false
        };

        var tenantContext = new FakeTenantContext
        {
            TenantId = tenantId,
            SchemaName = "tenant_test"
        };

        var service = new DiscountCouponService(
            repo,
            tenantContext);

        var dto = new CreateDiscountCouponDTO
        {
            EventId = Guid.NewGuid(),
            Code = "SAVE10",
            Type = DiscountType.Percentage,
            DiscountValue = 10,
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddDays(5),
            UsageLimit = 100
        };

        var result = await service.CreateAsync(dto, tenantId);

        Assert.Equal("SAVE10", result.Code);
        Assert.Equal(10, result.DiscountValue);

        Assert.NotNull(repo.AddedCoupon);
        Assert.Equal(tenantId, repo.AddedCoupon.TenantId);
        Assert.Equal("SAVE10", repo.AddedCoupon.Code);
        Assert.Equal(1, repo.SaveChangesCallCount);
    }

    [Fact]
    public async Task GetByIdAsync_When_Coupon_Is_Deleted_Should_Return_Null()
    {
        var repo = new FakeDiscountCouponRepository
        {
            CouponToReturn = new DiscountCoupon
            {
                Id = Guid.NewGuid(),
                Code = "OLD",
                IsDeleted = true
            }
        };

        var tenantContext = new FakeTenantContext();

        var service = new DiscountCouponService(
            repo,
            tenantContext);

        var result = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    private class FakeTenantContext : ITenantContext
    {
        public Guid TenantId { get; set; }

        public string? SchemaName { get; set; }
    }

    private class FakeDiscountCouponRepository : IDiscountCouponRepository
    {
        public bool ExistsByEventAndCode { get; set; }

        public DiscountCoupon? AddedCoupon { get; private set; }

        public DiscountCoupon? CouponToReturn { get; set; }

        public int SaveChangesCallCount { get; private set; }

        public Task<bool> ExistsByEventAndCodeAsync(
            Guid eventId,
            string code,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ExistsByEventAndCode);
        }

        public Task AddAsync(
            DiscountCoupon coupon,
            CancellationToken cancellationToken = default)
        {
            AddedCoupon = coupon;
            return Task.CompletedTask;
        }

        public Task<DiscountCoupon?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(CouponToReturn);
        }

        public Task<IReadOnlyList<DiscountCoupon>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<DiscountCoupon>>(
                new List<DiscountCoupon>());
        }

        public void Update(DiscountCoupon coupon)
        {
        }

        public void Delete(DiscountCoupon coupon)
        {
        }

        public Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.CompletedTask;
        }

        Task<List<DiscountCoupon>> IDiscountCouponRepository.GetAllAsync(
    CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<DiscountCoupon>());
        }

        public Task<List<DiscountCoupon>> GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<DiscountCoupon>());
        }

        public Task UpdateAsync(DiscountCoupon entity)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(DiscountCoupon entity)
        {
            return Task.CompletedTask;
        }
    }
}