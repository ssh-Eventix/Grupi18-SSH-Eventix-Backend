using Eventix.Application.DTOs.DiscountCoupons;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class DiscountCouponService : IDiscountCouponService
{
    private readonly IDiscountCouponRepository _repository;
    private readonly ITenantContext _tenantContext;

    public DiscountCouponService(IDiscountCouponRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<List<DiscountCouponResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Where(x => !x.IsDeleted).Select(Map).ToList();
    }

    public async Task<DiscountCouponResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null || entity.IsDeleted ? null : Map(entity);
    }

    public async Task<List<DiscountCouponResponseDTO>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByEventIdAsync(eventId, cancellationToken);
        return items.Where(x => !x.IsDeleted).Select(Map).ToList();
    }

    public async Task<DiscountCouponResponseDTO> CreateAsync(CreateDiscountCouponDTO dto, Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsByEventAndCodeAsync(dto.EventId, dto.Code, cancellationToken))
            throw new InvalidOperationException("A coupon with the same code already exists for this event");

        var entity = new DiscountCoupon
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,
            EventId = dto.EventId,
            Code = dto.Code,
            Type = dto.Type,
            DiscountValue = dto.DiscountValue,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo,
            UsageLimit = dto.UsageLimit,
            UsageCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateDiscountCouponDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        if ((entity.EventId != dto.EventId || !string.Equals(entity.Code, dto.Code, StringComparison.OrdinalIgnoreCase)) &&
            await _repository.ExistsByEventAndCodeAsync(dto.EventId, dto.Code, cancellationToken))
        {
            throw new InvalidOperationException("A coupon with the same code already exists for this event");
        }

        entity.EventId = dto.EventId;
        entity.Code = dto.Code;
        entity.Type = dto.Type;
        entity.DiscountValue = dto.DiscountValue;
        entity.ValidFrom = dto.ValidFrom;
        entity.ValidTo = dto.ValidTo;
        entity.UsageLimit = dto.UsageLimit;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static DiscountCouponResponseDTO Map(DiscountCoupon x) => new()
    {
        Id = x.Id,
        EventId = x.EventId,
        Code = x.Code,
        Type = x.Type,
        DiscountValue = x.DiscountValue,
        ValidFrom = x.ValidFrom,
        ValidTo = x.ValidTo,
        UsageLimit = x.UsageLimit,
        UsageCount = x.UsageCount,
        CreatedAtUtc = x.CreatedAtUtc,
        UpdatedAtUtc = x.UpdatedAtUtc
    };
}

