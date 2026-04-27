using Eventix.Application.DTOs.EventSessions;
using Eventix.Application.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EventSessionController : ControllerBase
{
    private readonly IEventSessionService _service;
    private readonly ITenantContext _tenantContext;

    public EventSessionController(IEventSessionService service, ITenantContext tenantContext)
    {
        _service = service;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventSessionResponseDTO>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(cancellationToken);
        var response = items.Where(x => x.TenantId == _tenantContext.TenantId).ToList();
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<EventSessionResponseDTO>> Create(CreateEventSessionDTO dto, CancellationToken cancellationToken)
    {
        var response = await _service.CreateAsync(dto, _tenantContext.TenantId, cancellationToken);
        return Ok(response);
    }
}