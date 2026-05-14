using Eventix.Application.DTOs.Auth;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Eventix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImpersonationController : ControllerBase
{
    private readonly IImpersonationService _impersonationService;

    public ImpersonationController(IImpersonationService impersonationService)
    {
        _impersonationService = impersonationService;
    }

    public class StartImpersonationRequest
    {
        [Required]
        public Guid TargetTenantId { get; set; }

        [Required]
        public Guid TargetPublicUserId { get; set; }

        [Range(1, 120)]
        public int Minutes { get; set; } = 10;

        [StringLength(500)]
        public string? Reason { get; set; }
    }

    public class StopImpersonationRequest
    {
        [Required]
        public Guid SessionId { get; set; }
    }

    [HttpPost("start")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(ImpersonationStartResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ImpersonationStartResult>> Start(
        [FromBody] StartImpersonationRequest dto,
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
            return Forbid(ex.Message);
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
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Stop(
        [FromBody] StopImpersonationRequest dto,
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