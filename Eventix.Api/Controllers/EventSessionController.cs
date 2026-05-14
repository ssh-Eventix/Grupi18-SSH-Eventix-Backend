using Eventix.Application.DTOs.EventSessions;
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EventSessionController : ControllerBase
{
    private readonly IEventSessionService _service;
    private readonly ITenantContext _tenantContext;

    public EventSessionController(
        IEventSessionService service,
        ITenantContext tenantContext)
    {
        _service = service;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventSessionResponseDTO>>> GetAll(CancellationToken cancellationToken)
    {
        var sessions = await _service.GetAllAsync(cancellationToken);
        return Ok(sessions);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventSessionResponseDTO>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var session = await _service.GetByIdAsync(id, cancellationToken);
        return session is null ? NotFound() : Ok(session);
    }

    [HttpPost]
    public async Task<ActionResult<EventSessionResponseDTO>> Create(
        [FromBody] CreateEventSessionDTO dto,
        CancellationToken cancellationToken)
    {
        var response = await _service.CreateAsync(
            dto,
            _tenantContext.TenantId,
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEventSessionDTO dto,
        CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(id, dto, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}