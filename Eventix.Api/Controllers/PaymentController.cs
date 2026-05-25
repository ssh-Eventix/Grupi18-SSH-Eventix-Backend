using Eventix.Application.DTOs.Payment;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Eventix.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IDistributedCache _cache;
        private readonly ITenantContext _tenantContext;

        public PaymentController(
            IPaymentService paymentService,
            IDistributedCache cache,
            ITenantContext tenantContext)
        {
            _paymentService = paymentService;
            _cache = cache;
            _tenantContext = tenantContext;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:ViewPayments")]
        public async Task<ActionResult> GetAll()
        {
            var cacheKey =
                $"tenant:{_tenantContext.TenantId}:payments:all";

            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData != null)
                return Content(cachedData, "application/json");

            var result = await _paymentService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:ViewPayments")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var cacheKey =
                $"tenant:{_tenantContext.TenantId}:payment:{id}";

            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData != null)
                return Content(cachedData, "application/json");

            var result = await _paymentService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("booking/{bookingId:guid}")]
        [Authorize(Policy = "Permission:ViewPayments")]
        public async Task<ActionResult> GetByBookingId(Guid bookingId)
        {
            var cacheKey =
                $"tenant:{_tenantContext.TenantId}:payments:booking:{bookingId}";

            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData != null)
                return Content(cachedData, "application/json");

            var result = await _paymentService.GetByBookingIdAsync(bookingId);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "Permission:CreatePayments")]
        public async Task<ActionResult> Create([FromBody] CreatePaymentDto request)
        {
            if (request == null)
                return BadRequest("Request is null");

            var result = await _paymentService.CreatePayment(request);

            await _cache.RemoveAsync(
                $"tenant:{_tenantContext.TenantId}:payments:all");

            await _cache.RemoveAsync(
                $"tenant:{_tenantContext.TenantId}:payments:booking:{request.BookingId}");

            return Ok(result);
        }
    }
}