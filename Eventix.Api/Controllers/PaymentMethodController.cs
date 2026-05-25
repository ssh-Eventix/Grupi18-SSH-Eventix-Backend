using Eventix.Application.DTOs.PaymentMethod;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethodController(IPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:ViewPaymentMethods")]
        public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetAll()
        {
            var result = await _paymentMethodService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:ViewPaymentMethods")]
        public async Task<ActionResult<PaymentMethodDto>> GetById(Guid id)
        {
            var result = await _paymentMethodService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "Permission:CreatePaymentMethods")]
        public async Task<ActionResult<PaymentMethodDto>> Create([FromBody] CreatePaymentMethodDto request)
        {
            if (request == null)
                return BadRequest("Request is null");

            var result = await _paymentMethodService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Permission:UpdatePaymentMethods")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentMethodDto request)
        {
            if (request == null)
                return BadRequest("Request is null");

            var result = await _paymentMethodService.UpdateAsync(id, request);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Permission:DeletePaymentMethods")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _paymentMethodService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id:guid}/activate")]
        [Authorize(Policy = "Permission:UpdatePaymentMethods")]
        public async Task<IActionResult> Activate(Guid id)
        {
            await _paymentMethodService.ActivateAsync(id);
            return NoContent();
        }

        [HttpPost("{id:guid}/deactivate")]
        [Authorize(Policy = "Permission:UpdatePaymentMethods")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _paymentMethodService.DeactivateAsync(id);
            return NoContent();
        }
    }
}