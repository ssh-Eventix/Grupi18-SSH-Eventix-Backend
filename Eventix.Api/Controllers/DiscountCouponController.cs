using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eventix.Application.DTOs.DiscountCoupons;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountCouponController : ControllerBase
    {
        private readonly IDiscountCouponService _service;
        private readonly ITenantContext _tenantContext;

        public DiscountCouponController(IDiscountCouponService service, ITenantContext tenantContext)
        {
            _service = service;
            _tenantContext = tenantContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiscountCouponResponseDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(cancellationToken);
            var response = items.Where(x => x.TenantId == _tenantContext.TenantId).ToList();
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DiscountCouponResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto is null || dto.TenantId != _tenantContext.TenantId)
                return NotFound();

            return Ok(dto);
        }

        [HttpGet("by-event/{eventId:guid}")]
        public async Task<ActionResult<List<DiscountCouponResponseDTO>>> GetByEventId(Guid eventId, CancellationToken cancellationToken)
        {
            var items = await _service.GetByEventIdAsync(eventId, cancellationToken);
            var response = items.Where(x => x.TenantId == _tenantContext.TenantId).ToList();
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<DiscountCouponResponseDTO>> Create([FromBody] CreateDiscountCouponDTO dto, CancellationToken cancellationToken)
        {
            var response = await _service.CreateAsync(dto, _tenantContext.TenantId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDiscountCouponDTO dto, CancellationToken cancellationToken)
        {
            var existing = await _service.GetByIdAsync(id, cancellationToken);
            if (existing is null || existing.TenantId != _tenantContext.TenantId)
                return NotFound();

            var updated = await _service.UpdateAsync(id, dto, cancellationToken);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var existing = await _service.GetByIdAsync(id, cancellationToken);
            if (existing is null || existing.TenantId != _tenantContext.TenantId)
                return NotFound();

            var deleted = await _service.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}

