using Eventix.Application.DTOs.Ai;

namespace Eventix.Application.Interfaces.Services;

public interface IAiService
{
    Task<AiResponseDTO> ChatAsync(AiChatRequestDTO request, Guid userId, CancellationToken ct);

    Task<AiResponseDTO> GenerateEventDescriptionAsync(
        GenerateEventDescriptionRequestDTO request,
        Guid userId,
        CancellationToken ct);

    Task<AiResponseDTO> GenerateReviewSummaryAsync(
        Guid eventId,
        Guid userId,
        CancellationToken ct);

    Task<AiResponseDTO> GenerateRecommendationsAsync(
        Guid userId,
        CancellationToken ct);

    Task<AiResponseDTO> GenerateMarketingAsync(
        GenerateMarketingRequestDTO request,
        Guid userId,
        CancellationToken ct);
}