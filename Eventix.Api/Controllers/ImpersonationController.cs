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
        public Guid TargetTenantUserId { get; set; }

        [Range(1, 120)]
        public int Minutes { get; set; } = 10;

        [StringLength(500)]
        public string? Reason { get; set; }
    }

    public class StartImpersonationResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAtUtc { get; set; }
        public Guid SessionId { get; set; }
    }

    [HttpPost("start")]
    [Authorize(Policy = global::ImpersonationAuthConstants.SuperAdminImpersonationPolicy)]
    [ProducesResponseType(typeof(StartImpersonationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StartImpersonationResponse>> Start([FromBody] StartImpersonationRequest dto, CancellationToken ct)
    {
        var subject = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(subject) || !Guid.TryParse(subject, out var impersonatorTenantUserId))
            return Unauthorized();
        if (dto.TargetTenantUserId == Guid.Empty)
            return BadRequest("TargetTenantUserId is required.");

        try
        {
            var result = await _impersonationService.StartImpersonationAsync(
                impersonatorTenantUserId,
                dto.TargetTenantUserId,
                dto.Minutes,
                dto.Reason,
                ct);

            return Ok(new StartImpersonationResponse
            {
                AccessToken = result.Token,
                AccessTokenExpiresAtUtc = result.ExpiresAtUtc,
                SessionId = result.SessionId
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already has an active impersonation session", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public class StopImpersonationRequest
    {
        public Guid SessionId { get; set; }
    }

    [HttpPost("stop")]
    [Authorize(Policy = global::ImpersonationAuthConstants.SuperAdminImpersonationPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Stop([FromBody] StopImpersonationRequest dto, CancellationToken ct)
    {
        try
        {
            if (dto.SessionId == Guid.Empty)
                return BadRequest("SessionId is required.");

            await _impersonationService.StopImpersonationAsync(dto.SessionId, ct);
            return Ok();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(ex.Message);
        }
    }
}
