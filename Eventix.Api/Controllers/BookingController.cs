using Eventix.Application.DTOs.Booking;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace Eventix.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IDistributedCache _cache;
        private readonly ITenantContext _tenantContext;

        public BookingController(
            IBookingService bookingService,
            IDistributedCache cache,
            ITenantContext tenantContext)
        {
                _bookingService = bookingService;
                _cache = cache;
                _tenantContext = tenantContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _bookingService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var result = await _bookingService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult> GetByUserId(Guid userId)
        {
            var result = await _bookingService.GetUserBookings(userId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy ="Permission:CreateBookings")]
        public async Task<ActionResult> Create([FromBody] CreateBookingRequest request)
        {
            if (request == null)
                return BadRequest("Request is null");

            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            if (!Guid.TryParse(userIdValue, out var userId))
                return Unauthorized("User id missing from token.");

            request.UserId = userId;

            var result = await _bookingService.CreateBooking(request);

            await _cache.RemoveAsync(
                $"tenant:{_tenantContext.TenantId}:tickettypes:event:{request.EventId}");

            await _cache.RemoveAsync(
                $"tenant:{_tenantContext.TenantId}:tickettypes:event:{request.EventId}:available");

            return Ok(result);
        }

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBookingStatusRequest request)
        {
            if (request == null)
                return BadRequest("Request is null");

            var result = await _bookingService.UpdateBookingStatus(id, request);

            return result ? Ok(true) : NotFound(false);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _bookingService.DeleteBooking(id);

            return result ? Ok(true) : NotFound(false);
        }
    }
}
