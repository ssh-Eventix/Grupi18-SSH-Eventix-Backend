using Eventix.Api.Helpers;
using Eventix.Application.DTOs.TicketType;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Eventix.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TicketTypeController : ControllerBase
    {
        private readonly ITicketTypeService _ticketTypeService;
        private readonly IDistributedCache _cache;
        private readonly ITenantContext _tenantContext;

        public TicketTypeController(
            ITicketTypeService ticketTypeService,
            IDistributedCache cache,
            ITenantContext tenantContext)
        {
            _ticketTypeService = ticketTypeService;
            _cache = cache;
            _tenantContext = tenantContext;
        }
        [HttpPost]
        [Authorize(Policy = "Permission:CreateTicketTypes")]
        public async Task<IActionResult> Create(
     [FromBody] CreateTicketTypeDto dto)
        {
            try
            {
                var result = await _ticketTypeService.CreateAsync(dto, _tenantContext.TenantId);

                await _cache.RemoveAsync(
                    $"tenant:{_tenantContext.TenantId}:tickettypes:event:{result.EventId}");

                await _cache.RemoveAsync(
                    $"tenant:{_tenantContext.TenantId}:tickettypes:event:{result.EventId}:available");

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("event/{eventId:guid}")]
        [Authorize(Policy = "Permission:ViewTicketTypes")]
        public async Task<IActionResult> GetByEventId(Guid eventId)
        {
            var cacheKey = $"tenant:{_tenantContext.TenantId}:tickettypes:event:{eventId}";

            var result = await CacheHelper.GetOrSetAsync(
                _cache,
                cacheKey,
                async () =>
                {
                    var ticketTypes = await _ticketTypeService.GetByEventIdAsync(eventId);

                    return ticketTypes.Select(t => new TicketTypeDto
                    {
                        Id = t.Id,
                        EventId = t.EventId,
                        EventSectionId = t.EventSectionId,
                        Name = t.Name,
                        Price = t.Price,
                        QuantityAvailable = t.QuantityAvailable,
                        SoldQuantity = t.SoldQuantity,
                        SaleStartDate = t.SaleStartDate,
                        SaleEndDate = t.SaleEndDate
                    }).ToList();
                },
                TimeSpan.FromMinutes(5));

            return Ok(result);
        }

        [HttpGet("event/{eventId:guid}/available")]
        [Authorize(Policy = "Permission:ViewTicketTypes")]
        public async Task<IActionResult> GetAvailableByEventId(Guid eventId)
        {
            var cacheKey = $"tenant:{_tenantContext.TenantId}:tickettypes:event:{eventId}:available";

            var result = await CacheHelper.GetOrSetAsync(
                _cache,
                cacheKey,
                async () =>
                {
                    var ticketTypes = await _ticketTypeService.GetAvailableByEventIdAsync(eventId);

                    return ticketTypes.Select(t => new TicketTypeDto
                    {
                        Id = t.Id,
                        EventId = t.EventId,
                        EventSectionId = t.EventSectionId,
                        Name = t.Name,
                        Price = t.Price,
                        QuantityAvailable = t.QuantityAvailable,
                        SoldQuantity = t.SoldQuantity,
                        SaleStartDate = t.SaleStartDate,
                        SaleEndDate = t.SaleEndDate
                    }).ToList();
                },
                TimeSpan.FromMinutes(2));

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:ViewTicketTypes")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var cacheKey = $"tenant:{_tenantContext.TenantId}:tickettype:{id}";

            var result = await CacheHelper.GetOrSetAsync(
                _cache,
                cacheKey,
                async () =>
                {
                    var ticketType = await _ticketTypeService.GetByIdAsync(id);

                    if (ticketType == null)
                        return null;

                    return new TicketTypeDto
                    {
                        Id = ticketType.Id,
                        EventId = ticketType.EventId,
                        EventSectionId = ticketType.EventSectionId,
                        Name = ticketType.Name,
                        Price = ticketType.Price,
                        QuantityAvailable = ticketType.QuantityAvailable,
                        SoldQuantity = ticketType.SoldQuantity,
                        SaleStartDate = ticketType.SaleStartDate,
                        SaleEndDate = ticketType.SaleEndDate
                    };
                },
                TimeSpan.FromMinutes(5));

            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("public/event/{eventId:guid}/available")]
        [AllowAnonymous]
        public async Task<IActionResult> PublicGetAvailableByEventId(Guid eventId)
        {
            var ticketTypes = await _ticketTypeService.GetAvailableByEventIdAsync(eventId);

            var result = ticketTypes.Select(t => new TicketTypeDto
            {
                Id = t.Id,
                EventId = t.EventId,
                EventSectionId = t.EventSectionId,
                Name = t.Name,
                Price = t.Price,
                QuantityAvailable = t.QuantityAvailable,
                SoldQuantity = t.SoldQuantity,
                SaleStartDate = t.SaleStartDate,
                SaleEndDate = t.SaleEndDate
            }).ToList();

            return Ok(result);
        }
    }


}
