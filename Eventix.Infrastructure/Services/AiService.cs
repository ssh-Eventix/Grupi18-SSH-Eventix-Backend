using Eventix.Application.DTOs.Ai;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Services;

public class AiService : IAiService
{
    private readonly IOllamaClient _ollamaClient;
    private readonly IAIRequestLogRepository _logRepository;
    private readonly TenantDbContext _context;

    public AiService(
        IOllamaClient ollamaClient,
        IAIRequestLogRepository logRepository,
        TenantDbContext context)
    {
        _ollamaClient = ollamaClient;
        _logRepository = logRepository;
        _context = context;
    }

    public async Task<AiResponseDTO> ChatAsync(
        AiChatRequestDTO request,
        Guid userId,
        CancellationToken ct)
    {
        var prompt = $"""
        You are an assistant for an event management platform called Eventix.
        Answer clearly and professionally.

        User question:
        {request.Prompt}
        """;

        return await ExecuteAiRequestAsync(
            userId,
            prompt,
            AIRequestType.Chat,
            ct);
    }

    public async Task<AiResponseDTO> GenerateEventDescriptionAsync(
        GenerateEventDescriptionRequestDTO request,
        Guid userId,
        CancellationToken ct)
    {
        var prompt = $"""
        You are a senior event copywriter for Eventix.
        Write a detailed, polished event description that can be published directly on an event ticketing page.

        Event details:
        - Title: {request.Title}
        - Category: {request.Category}
        - Location/Venue: {request.Location}
        - Organizer: {request.OrganizerName}
        - Starts: {request.StartUtc}
        - Ends: {request.EndUtc}
        - Pricing: {(request.IsFree ? "Free event" : $"Paid event, currency {request.Currency}")}

        Requirements:
        - Write in clear professional English.
        - Do not invent specific artists, speakers, prices, sponsors, or schedules that were not provided.
        - Make it engaging and useful for attendees.
        - Mention the event mood, audience value, venue/location, and what attendees can expect.
        - Return only the final event description, no bullet labels and no explanation.
        - Length: 120 to 180 words.
        """;

        return await ExecuteAiRequestAsync(
            userId,
            prompt,
            AIRequestType.Summarization,
            ct);
    }

    public async Task<AiResponseDTO> GenerateReviewSummaryAsync(
        Guid eventId,
        Guid userId,
        CancellationToken ct)
    {
        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .Select(x => new
            {
                x.Rating,
                x.Comment
            })
            .ToListAsync(ct);

        if (!reviews.Any())
        {
            return new AiResponseDTO
            {
                Response = "No reviews found for this event.",
                TokensUsed = 0
            };
        }

        var reviewsText = string.Join("\n", reviews.Select(r =>
            $"Rating: {r.Rating}, Comment: {r.Comment}"));

        var prompt = $"""
        Analyze these event reviews.

        Reviews:
        {reviewsText}

        Return:
        - general sentiment: positive, neutral or negative
        - short summary
        - what attendees liked
        - what should be improved
        - keep the full answer under 1000 words
        - use clear short sections and avoid unnecessary repetition
        """;

        return await ExecuteAiRequestAsync(
            userId,
            prompt,
            AIRequestType.TextAnalysis,
            ct);
    }

    public async Task<AiResponseDTO> GenerateRecommendationsAsync(
        Guid userId,
        CancellationToken ct)
    {
        var bookings = await _context.Bookings
            .AsNoTracking()
            .Include(x => x.Event)
            .ThenInclude(x => x.EventCategory)
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);

        if (!bookings.Any())
        {
            return new AiResponseDTO
            {
                Response = "No previous bookings found. Try recommending popular upcoming events.",
                TokensUsed = 0
            };
        }

        var bookingText = string.Join("\n", bookings.Select(b =>
            $"Event: {b.Event.Title}, Category: {b.Event.EventCategory.Name}, Date: {b.Event.StartUtc}"));

        var prompt = $"""
        Based on the user's previous bookings, generate smart event recommendations.

        Previous bookings:
        {bookingText}

        Return 3 recommendations in this style:
        "Because you attended ..., you may also like ..."
        """;

        return await ExecuteAiRequestAsync(
            userId,
            prompt,
            AIRequestType.Recommendation,
            ct);
    }

    public async Task<AiResponseDTO> GenerateMarketingAsync(
        GenerateMarketingRequestDTO request,
        Guid userId,
        CancellationToken ct)
    {
        var prompt = $"""
            You are an experienced event marketing specialist.

            Create a high-quality event description suitable for display on an Eventbrite-style event page.

            Event Title:
            {request.EventTitle}

            Event Information:
            {request.EventDescription}

            Rules:
            - Use professional marketing language.
            - Create excitement and interest.
            - Write 150-250 words.
            - Use clear paragraphs.
            - No markdown formatting.
            - No bullet points.
            - No headings.
            - No placeholders.
            - No pricing information.
            - No contact instructions.
            - Return only the event description.

            """;

        return await ExecuteAiRequestAsync(
            userId,
            prompt,
            AIRequestType.Summarization,
            ct);
    }

    private async Task<AiResponseDTO> ExecuteAiRequestAsync(
        Guid userId,
        string prompt,
        AIRequestType requestType,
        CancellationToken ct)
    {
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == userId, ct);

        if (!userExists)
            throw new UnauthorizedAccessException("User does not exist in this tenant.");

        var log = new AIRequestLog
        {
            UserId = userId,
            Prompt = prompt,
            RequestType = requestType,
            Status = AIRequestStatus.Pending,
            TokensUsed = 0
        };

        await _logRepository.AddAsync(log);
        await _logRepository.SaveChangesAsync();

        try
        {
            var aiResponse = await _ollamaClient.GenerateAsync(prompt, ct);

            log.ResponseSummary = aiResponse.Length > 4000
                ? aiResponse[..4000]
                : aiResponse;

            log.Status = AIRequestStatus.Completed;
            log.TokensUsed = EstimateTokens(prompt + aiResponse);

            _logRepository.Update(log);
            await _logRepository.SaveChangesAsync();

            return new AiResponseDTO
            {
                Response = aiResponse,
                TokensUsed = log.TokensUsed
            };
        }
        catch
        {
            log.Status = AIRequestStatus.Failed;

            _logRepository.Update(log);
            await _logRepository.SaveChangesAsync();

            throw;
        }
    }

    private static int EstimateTokens(string text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? 0
            : text.Length / 4;
    }
}
