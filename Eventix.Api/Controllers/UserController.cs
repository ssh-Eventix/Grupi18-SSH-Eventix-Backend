using Eventix.Application.DTOs.User;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:ViewUsers")]
    public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAll(CancellationToken ct)
    {
        var users = await _service.GetAllAsync(ct);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:ViewUsers")]
    public async Task<ActionResult<UserResponseDTO>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpGet("by-email")]
    [Authorize(Policy = "Permission:ViewUsers")]
    public async Task<ActionResult<UserResponseDTO>> GetByEmail([FromQuery] string email, CancellationToken ct)
    {
        var dto = await _service.GetByEmailAsync(email, ct);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:CreateUsers")]
    public async Task<ActionResult<UserResponseDTO>> Create(
        [FromBody] CreateUserDTO dto,
        CancellationToken ct)
    {
        var response = await _service.CreateAsync(dto, ct);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:UpdateUsers")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserDTO dto,
        CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, dto, ct);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:DeleteUsers")]
    public async Task<IActionResult> Delete(
    Guid id,
    CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);

        return deleted
            ? NoContent()
            : NotFound();
    }

}