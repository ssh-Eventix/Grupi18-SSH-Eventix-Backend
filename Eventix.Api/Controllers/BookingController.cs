using Eventix.Application.DTOs.Booking;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
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
        public async Task<ActionResult> Create([FromBody] CreateBookingRequest request)
        {
            if (request == null)
                return BadRequest("Request is null");

            var result = await _bookingService.CreateBooking(request);

            return Ok(result);
        }
    }
}
