using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eventix.Application.DTOs.DiscountCoupons;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountCouponController : ControllerBase
    {
        private readonly IDiscountCouponRepository _repository;
        private readonly ITenantContext _tenantContext;

        public DiscountCouponController(IDiscountCouponRepository repository, ITenantContext tenantContext)
        {
            _repository = repository;
            _tenantContext = tenantContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiscountCouponResponseDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            var response = items.Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
                .Select(x => new DiscountCouponResponseDTO
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
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
                })
                .ToList();

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DiscountCouponResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
                return NotFound();

            var dto = new DiscountCouponResponseDTO
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                EventId = entity.EventId,
                Code = entity.Code,
                Type = entity.Type,
                DiscountValue = entity.DiscountValue,
                ValidFrom = entity.ValidFrom,
                ValidTo = entity.ValidTo,
                UsageLimit = entity.UsageLimit,
                UsageCount = entity.UsageCount,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            };

            return Ok(dto);
        }

        [HttpGet("by-event/{eventId:guid}")]
        public async Task<ActionResult<List<DiscountCouponResponseDTO>>> GetByEventId(Guid eventId, CancellationToken cancellationToken)
        {
            var items = await _repository.GetByEventIdAsync(eventId, cancellationToken);
            var response = items.Where(x => x.TenantId == _tenantContext.TenantId && !x.IsDeleted)
                .Select(x => new DiscountCouponResponseDTO
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
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
                })
                .ToList();

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<DiscountCouponResponseDTO>> Create([FromBody] CreateDiscountCouponDTO dto, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByEventAndCodeAsync(dto.EventId, dto.Code, cancellationToken))
                return Conflict("A coupon with the same code already exists for this event");

            var entity = new DiscountCoupon
            {
                Id = Guid.NewGuid(),
                EventId = dto.EventId,
                Code = dto.Code,
                Type = dto.Type,
                DiscountValue = dto.DiscountValue,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                UsageLimit = dto.UsageLimit,
                UsageCount = 0,
                TenantId = _tenantContext.TenantId,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            var response = new DiscountCouponResponseDTO
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                EventId = entity.EventId,
                Code = entity.Code,
                Type = entity.Type,
                DiscountValue = entity.DiscountValue,
                ValidFrom = entity.ValidFrom,
                ValidTo = entity.ValidTo,
                UsageLimit = entity.UsageLimit,
                UsageCount = entity.UsageCount,
                CreatedAtUtc = entity.CreatedAtUtc
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDiscountCouponDTO dto, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
                return NotFound();

            // If code or event changed, ensure uniqueness
            if ((entity.EventId != dto.EventId || !string.Equals(entity.Code, dto.Code, StringComparison.OrdinalIgnoreCase)) &&
                await _repository.ExistsByEventAndCodeAsync(dto.EventId, dto.Code, cancellationToken))
            {
                return Conflict("A coupon with the same code already exists for this event");
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

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity is null || entity.TenantId != _tenantContext.TenantId || entity.IsDeleted)
                return NotFound();

            entity.IsDeleted = true;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}

