using Eventix.Application.DTOs.TicketType;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketTypeController : ControllerBase
    {
        private readonly ITicketTypeService _ticketTypeService;

        public TicketTypeController(ITicketTypeService ticketTypeService)
        {
            _ticketTypeService = ticketTypeService;
        }

        [HttpGet("event/{eventId:guid}")]
        public async Task<IActionResult> GetByEventId(Guid eventId)
        {
            var ticketTypes = await _ticketTypeService.GetByEventIdAsync(eventId);

            var result = ticketTypes.Select(t => new TicketTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                Price = t.Price,
                QuantityAvailable = t.QuantityAvailable,
                SaleStartDate = t.SaleStartDate,
                SaleEndDate = t.SaleEndDate
            });

            return Ok(result);
        }

        [HttpGet("event/{eventId:guid}/available")]
        public async Task<IActionResult> GetAvailableByEventId(Guid eventId)
        {
            var ticketTypes = await _ticketTypeService.GetAvailableByEventIdAsync(eventId);

            var result = ticketTypes.Select(t => new TicketTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                Price = t.Price,
                QuantityAvailable = t.QuantityAvailable,
                SaleStartDate = t.SaleStartDate,
                SaleEndDate = t.SaleEndDate
            });

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticketType = await _ticketTypeService.GetByIdAsync(id);

            if (ticketType == null)
                return NotFound();

            var result = new TicketTypeDto
            {
                Id = ticketType.Id,
                Name = ticketType.Name,
                Price = ticketType.Price,
                QuantityAvailable = ticketType.QuantityAvailable,
                SaleStartDate = ticketType.SaleStartDate,
                SaleEndDate = ticketType.SaleEndDate
            };

            return Ok(result);
        }

    }
}
