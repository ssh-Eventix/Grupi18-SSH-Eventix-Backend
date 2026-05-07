using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);

            if (ticket == null)
                return NotFound("Ticket not found");

            return Ok(ticket);
        }

        [HttpGet("code/{ticketCode}")]
        public async Task<IActionResult> GetByCode(string ticketCode)
        {
            var ticket = await _ticketService.GetByCodeAsync(ticketCode);

            if (ticket == null)
                return NotFound("Ticket not found");

            return Ok(ticket);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] string ticketCode)
        {
            var isValid = await _ticketService.ValidateTicketAsync(ticketCode);

            if (!isValid)
                return BadRequest("Invalid or inactive ticket");

            return Ok(new { message = "Ticket is valid" });
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] string ticketCode)
        {
            try
            {
                await _ticketService.CheckInAsync(ticketCode);

                return Ok(new
                {
                    message = "Ticket successfully checked in"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
