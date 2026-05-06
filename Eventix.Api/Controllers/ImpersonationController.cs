using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public Guid TargetTenantUserId { get; set; }
        public int Minutes { get; set; } = 10;
        public string? Reason { get; set; }
    }

    public class StartImpersonationResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAtUtc { get; set; }
        public Guid SessionId { get; set; }
    }

    [HttpPost("start")]
    [Authorize(Policy = "SuperAdminImpersonationOnly")]
    public async Task<ActionResult<StartImpersonationResponse>> Start([FromBody] StartImpersonationRequest dto, CancellationToken ct)
    {
        var subject = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(subject) || !Guid.TryParse(subject, out var impersonatorTenantUserId))
            return Unauthorized();

        var (token, expires, sessionId) = await _impersonationService.StartImpersonationAsync(impersonatorTenantUserId, dto.TargetTenantUserId, dto.Minutes, dto.Reason, ct);

        return Ok(new StartImpersonationResponse
        {
            AccessToken = token,
            AccessTokenExpiresAtUtc = expires,
            SessionId = sessionId
        });
    }

    public class StopImpersonationRequest
    {
        public Guid SessionId { get; set; }
    }

    [HttpPost("stop")]
    [Authorize(Policy = "SuperAdminImpersonationOnly")]
    public async Task<IActionResult> Stop([FromBody] StopImpersonationRequest dto, CancellationToken ct)
    {
        var subject = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? actorTenantUserId = null;
        if (!string.IsNullOrEmpty(subject) && Guid.TryParse(subject, out var tenantId))
            actorTenantUserId = tenantId;

        await _impersonationService.StopImpersonationAsync(dto.SessionId, actorTenantUserId, null, ct);
        return Ok();
    }
}

