using Eventix.Application.DTOs.VenueSections;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenueSectionsController : ControllerBase
{
    private readonly IVenueSectionService _service;

    public VenueSectionsController(IVenueSectionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("venue/{venueId:guid}")]
    public async Task<IActionResult> GetByVenue(Guid venueId)
        => Ok(await _service.GetByVenueIdAsync(venueId));

    [HttpPost]
    public async Task<IActionResult> Create(CreateVenueSectionDTO dto)
        => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateVenueSectionDTO dto)
        => await _service.UpdateAsync(id, dto) ? Ok() : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
