using Eventix.Application.DTOs.Notifications;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationController(INotificationService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewNotifications")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id}")]
    [Authorize(Policy = "Permission:ViewNotifications")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var res = await _service.GetByIdAsync(id, ct);
        return res == null ? NotFound() : Ok(res);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:ManageNotifications")]
    public async Task<IActionResult> Create(CreateNotificationDTO dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));
}