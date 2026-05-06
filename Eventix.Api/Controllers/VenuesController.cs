using Eventix.Application.DTOs.Venues;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _service;

    public VenuesController(IVenueService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Buyer,SuperAdmin")]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Buyer,SuperAdmin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var venue = await _service.GetByIdAsync(id);
        return venue == null ? NotFound() : Ok(venue);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create(CreateVenueDTO dto)
        => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(Guid id, UpdateVenueDTO dto)
        => await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
