using Eventix.Application.DTOs.Auth;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImpersonationController : ControllerBase
{
    private readonly IImpersonationService _impersonationService;

    public ImpersonationController(IImpersonationService impersonationService)
    {
        _impersonationService = impersonationService;
    }

    [HttpPost("start")]
    [Authorize(Policy = "Permission:ImpersonateTenant")]
    [ProducesResponseType(typeof(ImpersonationStartResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ImpersonationStartResult>> Start(
        [FromBody] StartImpersonationRequestDTO dto,
        CancellationToken ct)
    {
        var subject = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(subject) ||
            !Guid.TryParse(subject, out var superAdminPublicUserId))
        {
            return Unauthorized();
        }

        if (dto.TargetTenantId == Guid.Empty)
            return BadRequest("TargetTenantId is required.");

        if (dto.TargetPublicUserId == Guid.Empty)
            return BadRequest("TargetPublicUserId is required.");

        if (dto.Minutes <= 0 || dto.Minutes > 120)
            return BadRequest("Minutes must be between 1 and 120.");

        try
        {
            var result = await _impersonationService.StartImpersonationAsync(
                superAdminPublicUserId,
                dto.TargetTenantId,
                dto.TargetPublicUserId,
                dto.Minutes,
                dto.Reason,
                ct);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("already active", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("active impersonation", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("stop")]
    [Authorize(Policy = "Permission:ImpersonateTenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Stop(
        [FromBody] StopImpersonationRequestDTO dto,
        CancellationToken ct)
    {
        if (dto.SessionId == Guid.Empty)
            return BadRequest("SessionId is required.");

        try
        {
            await _impersonationService.StopImpersonationAsync(dto.SessionId, ct);

            return Ok(new
            {
                message = "Impersonation stopped successfully."
            });
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(ex.Message);
        }
    }
}