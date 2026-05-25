using Eventix.Application.DTOs.Staff;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:ViewUsers")]
        public async Task<ActionResult<List<StaffResponseDTO>>> GetAll(CancellationToken ct)
        {
            var staff = await _staffService.GetAllAsync(ct);
            return Ok(staff);
        }

        [HttpPost]
        [Authorize(Policy = "Permission:CreateUsers")]
        public async Task<ActionResult<StaffResponseDTO>> Create(CreateStaffDTO dto, CancellationToken ct)
        {
            try
            {
                var staff = await _staffService.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(GetAll), new { id = staff.Id }, staff);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}/deactivate")]
        [Authorize(Policy = "Permission:UpdateUsers")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            var result = await _staffService.DeactivateAsync(id, ct);
            return result ? NoContent() : NotFound();
        }
    }
}
