using Eventix.Application.DTOs.Ai;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eventix.Api.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;

    public AiController(IAiService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("chat")]
    [Authorize(Policy = "Permission:UseAI")]
    public async Task<IActionResult> Chat(
        [FromBody] AiChatRequestDTO request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest("Prompt is required.");

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("Invalid user token.");

        return Ok(await _aiService.ChatAsync(request, userId, ct));
    }

    [HttpPost("buyer/chat")]
    [Authorize]
    public async Task<IActionResult> BuyerChat(
        [FromBody] AiChatRequestDTO request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest("Prompt is required.");

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("Invalid user token.");

        return Ok(await _aiService.BuyerChatAsync(request, userId, ct));
    }

    [HttpGet("recommendations")]
    [Authorize(Policy = "Permission:UseAI")]
    public async Task<IActionResult> Recommendations(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("Invalid user token.");

        return Ok(await _aiService.GenerateRecommendationsAsync(userId, ct));
    }

    [HttpGet("buyer/recommendations")]
    [Authorize]
    public async Task<IActionResult> BuyerRecommendations(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("Invalid user token.");

        return Ok(await _aiService.GenerateBuyerRecommendationsAsync(userId, ct));
    }

    [HttpPost("generate-event-description")]
    [Authorize(Policy = "Permission:UseAI")]
    public async Task<IActionResult> GenerateEventDescription(
        [FromBody] GenerateEventDescriptionRequestDTO request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("Invalid user token.");

        return Ok(await _aiService.GenerateEventDescriptionAsync(request, userId, ct));
    }

    [HttpGet("review-summary/{eventId:guid}")]
    [Authorize(Policy = "Permission:ViewReviews")]
    public async Task<IActionResult> ReviewSummary(
        Guid eventId,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("Invalid user token.");

        return Ok(await _aiService.GenerateReviewSummaryAsync(eventId, userId, ct));
    }

    [HttpPost("generate-marketing")]
    [Authorize(Policy = "Permission:UseAI")]
    public async Task<IActionResult> GenerateMarketing(
        [FromBody] GenerateMarketingRequestDTO request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("Invalid user token.");

        return Ok(await _aiService.GenerateMarketingAsync(request, userId, ct));
    }

    private Guid GetCurrentUserId()
    {
        var value =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.TryParse(value, out var userId)
            ? userId
            : Guid.Empty;
    }

    [HttpGet("debug-claims")]
    [Authorize]
    public IActionResult DebugClaims()
    {
        return Ok(User.Claims.Select(c => new
        {
            c.Type,
            c.Value
        }));
    }
}
