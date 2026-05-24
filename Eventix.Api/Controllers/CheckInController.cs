using Eventix.Application.DTOs.CheckIns;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckInController : ControllerBase
{
    private readonly ICheckInService _service;

    public CheckInController(ICheckInService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewCheckIns")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id}")]
    [Authorize(Policy = "Permission:ViewCheckIns")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var res = await _service.GetByIdAsync(id, ct);
        return res == null ? NotFound() : Ok(res);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:ManageCheckIns")]
    public async Task<IActionResult> Create(CreateCheckInDTO dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));
}