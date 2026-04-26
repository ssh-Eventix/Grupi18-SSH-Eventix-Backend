using Eventix.Application.DTOs.EventCategories;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventCategoriesController : ControllerBase
{
    private readonly IEventCategoryService _eventCategoryService;

    public EventCategoriesController(IEventCategoryService eventCategoryService)
    {
        _eventCategoryService = eventCategoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventCategoryResponseDTO>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _eventCategoryService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventCategoryResponseDTO>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _eventCategoryService.GetByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<EventCategoryResponseDTO>> Create(
        [FromBody] CreateEventCategoryDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _eventCategoryService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEventCategoryDTO dto,
        CancellationToken cancellationToken)
    {
        var updated = await _eventCategoryService.UpdateAsync(id, dto, cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _eventCategoryService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}