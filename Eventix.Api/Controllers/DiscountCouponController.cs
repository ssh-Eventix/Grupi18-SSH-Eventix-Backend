using Eventix.Api.Helpers;
using Eventix.Application.DTOs.DiscountCoupons;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Eventix.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountCouponController : ControllerBase
    {
        private readonly IDiscountCouponService _service;
        private readonly ITenantContext _tenantContext;
        private readonly IDistributedCache _cache;

        public DiscountCouponController(
        IDiscountCouponService service,
        ITenantContext tenantContext,
        IDistributedCache cache)
        {
            _service = service;
            _tenantContext = tenantContext;
            _cache = cache;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:ViewDiscountCoupons")]
        public async Task<ActionResult<IEnumerable<DiscountCouponResponseDTO>>> GetAll(
       CancellationToken cancellationToken)
        {
            var cacheKey = $"tenant:{_tenantContext.TenantId}:discountcoupons:all";

            var items = await CacheHelper.GetOrSetAsync(
                _cache,
                cacheKey,
                () => _service.GetAllAsync(cancellationToken),
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:ViewDiscountCoupons")]
        public async Task<ActionResult<DiscountCouponResponseDTO>> GetById(
        Guid id,
        CancellationToken cancellationToken)
        {
            var cacheKey = $"tenant:{_tenantContext.TenantId}:discountcoupon:{id}";

            var dto = await CacheHelper.GetOrSetAsync(
                _cache,
                cacheKey,
                () => _service.GetByIdAsync(id, cancellationToken),
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpGet("by-event/{eventId:guid}")]
        [Authorize(Policy = "Permission:ViewDiscountCoupons")]
        public async Task<ActionResult<List<DiscountCouponResponseDTO>>> GetByEventId(
       Guid eventId,
       CancellationToken cancellationToken)
        {
            var cacheKey = $"tenant:{_tenantContext.TenantId}:discountcoupons:event:{eventId}";

            var items = await CacheHelper.GetOrSetAsync(
                _cache,
                cacheKey,
                () => _service.GetByEventIdAsync(eventId, cancellationToken),
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return Ok(items);
        }

        [HttpPost]
        [Authorize(Policy = "Permission:CreateDiscountCoupons")]
        public async Task<ActionResult<DiscountCouponResponseDTO>> Create(
        [FromBody] CreateDiscountCouponDTO dto,
        CancellationToken cancellationToken)
        {
            var response = await _service.CreateAsync(dto, _tenantContext.TenantId, cancellationToken);

            await _cache.RemoveAsync(
                $"tenant:{_tenantContext.TenantId}:discountcoupons:all",
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Permission:UpdateDiscountCoupons")]
        public async Task<IActionResult> Update(
         Guid id,
         [FromBody] UpdateDiscountCouponDTO dto,
         CancellationToken cancellationToken)
        {
            var updated = await _service.UpdateAsync(id, dto, cancellationToken);

            if (!updated)
                return NotFound();

            await _cache.RemoveAsync(
                $"tenant:{_tenantContext.TenantId}:discountcoupons:all",
                cancellationToken);

            await _cache.RemoveAsync(
                $"tenant:{_tenantContext.TenantId}:discountcoupon:{id}",
                cancellationToken);

            return NoContent();
        }


        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Permission:DeleteDiscountCoupons")]
        public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            await _cache.RemoveAsync(
                $"tenant:{_tenantContext.TenantId}:discountcoupons:all",
                cancellationToken);

            await _cache.RemoveAsync(
                $"tenant:{_tenantContext.TenantId}:discountcoupon:{id}",
                cancellationToken);

            return NoContent();
        }

        [HttpPost("validate")]
        [AllowAnonymous]
        public async Task<ActionResult<ValidateDiscountCouponResponseDTO>> Validate(
    [FromBody] ValidateDiscountCouponDTO dto,
    CancellationToken cancellationToken)
        {
            return Ok(await _service.ValidateAsync(dto, cancellationToken));
        }
    }
}

