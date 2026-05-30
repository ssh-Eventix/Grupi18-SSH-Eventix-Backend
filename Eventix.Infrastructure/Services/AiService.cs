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
    private readonly IPublicEventService _publicEventService;

    public AiService(
        IOllamaClient ollamaClient,
        IAIRequestLogRepository logRepository,
        TenantDbContext context,
        IPublicEventService publicEventService)
    {
        _ollamaClient = ollamaClient;
        _logRepository = logRepository;
        _context = context;
        _publicEventService = publicEventService;
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

        var response = await _ollamaClient.GenerateAsync(prompt, ct);

        return new AiResponseDTO
        {
            Response = response,
            TokensUsed = EstimateTokens(prompt + response)
        };
    }

    public async Task<AiResponseDTO> BuyerChatAsync(
        AiChatRequestDTO request,
        Guid userId,
        CancellationToken ct)
    {
        var events = await _publicEventService.GetAllPublicAsync(null, ct);
        var isComparisonRequest = IsComparisonRequest(request.Prompt);
        var requestedCategory = DetectRequestedCategory(request.Prompt);
        var requestedCity = DetectRequestedCity(request.Prompt);
        var requestedDateRange = DetectRequestedDateRange(request.Prompt);
        var asksForFreeEvents = request.Prompt.Contains("free", StringComparison.OrdinalIgnoreCase);

        var filteredEvents = events
            .Where(x => x.StartUtc > DateTime.UtcNow);

        if (!isComparisonRequest && !string.IsNullOrWhiteSpace(requestedCategory))
        {
            filteredEvents = filteredEvents.Where(x =>
                string.Equals(x.EventCategoryName, requestedCategory, StringComparison.OrdinalIgnoreCase));
        }

        if (!isComparisonRequest && !string.IsNullOrWhiteSpace(requestedCity))
        {
            filteredEvents = filteredEvents.Where(x =>
                (x.VenueName ?? string.Empty).Contains(requestedCity, StringComparison.OrdinalIgnoreCase) ||
                (x.Title ?? string.Empty).Contains(requestedCity, StringComparison.OrdinalIgnoreCase));
        }

        if (!isComparisonRequest && asksForFreeEvents)
        {
            filteredEvents = filteredEvents.Where(x => x.IsFree);
        }

        if (!isComparisonRequest && requestedDateRange is not null)
        {
            filteredEvents = filteredEvents.Where(x =>
                x.StartUtc >= requestedDateRange.Value.StartUtc &&
                x.StartUtc < requestedDateRange.Value.EndUtc);
        }

        var matchingEvents = filteredEvents
            .OrderBy(x => x.StartUtc)
            .Take(20)
            .ToList();

        if (!matchingEvents.Any())
        {
            return new AiResponseDTO
            {
                Response = "No matching upcoming events were found.",
                TokensUsed = 0
            };
        }

        if (!isComparisonRequest &&
            (!string.IsNullOrWhiteSpace(requestedCategory) ||
             !string.IsNullOrWhiteSpace(requestedCity) ||
             asksForFreeEvents ||
             requestedDateRange is not null))
        {
            return new AiResponseDTO
            {
                Response = BuildDirectEventAnswer(matchingEvents, requestedCategory, asksForFreeEvents),
                TokensUsed = 0
            };
        }

        var upcomingEvents = matchingEvents
            .Select(x =>
                $"Title: {x.Title}\nCategory: {x.EventCategoryName}\nVenue: {x.VenueName}\nStarts: {x.StartUtc:u}\nPrice: {(x.IsFree ? "Free" : $"Paid in {x.Currency}")}\nTenant: {x.TenantSlug}")
            .ToList();

        var prompt = $"""
        You are the Eventix buyer assistant.

        Help buyers discover events, compare categories, and choose what to attend.
        Current UTC date: {DateTime.UtcNow:u}

        Available upcoming events:
        {string.Join("\n---\n", upcomingEvents)}

        Buyer question:
        {request.Prompt}

        Rules:
        - Answer based on the provided events.
        - Recommend only events from the provided list.
        - Comparing two or more provided events is allowed. If the buyer asks to compare events, compare their category, venue, start date, price, and who each event is best for.
        - Treat the category as only the value after "Category:". Do not infer category from the title.
        - If the buyer asks for a category, return only events where "Category:" exactly matches that category.
        - If the buyer asks for free events, return only events where "Price:" is exactly Free.
        - If the buyer asks for a city or venue, return only events whose "Venue:" or "Title:" contains that city or venue.
        - If the buyer asks for today, tomorrow, this week, or next week, return only events in that exact date range.
        - If no provided event matches the buyer's request, say that no matching upcoming events were found.
        - Do not invent prices, speakers, or unavailable details.
        - If the question is not about Eventix or events, politely guide the buyer back to event discovery.
        - Keep the answer concise and friendly.
        """;

        var response = await _ollamaClient.GenerateAsync(prompt, ct);

        return new AiResponseDTO
        {
            Response = response,
            TokensUsed = EstimateTokens(prompt + response)
        };
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

        var response = await _ollamaClient.GenerateAsync(prompt, ct);

        return new AiResponseDTO
        {
            Response = response,
            TokensUsed = EstimateTokens(prompt + response)
        };
    }

    public async Task<AiResponseDTO> GenerateBuyerRecommendationsAsync(
        Guid userId,
        CancellationToken ct)
    {
        var events = await _publicEventService.GetAllPublicAsync(null, ct);

        var upcomingEvents = events
            .Where(x => x.StartUtc > DateTime.UtcNow)
            .OrderBy(x => x.StartUtc)
            .Take(12)
            .Select(x =>
                $"- {x.Title}, category: {x.EventCategoryName}, venue: {x.VenueName}, starts: {x.StartUtc:u}, price: {(x.IsFree ? "Free" : $"Paid in {x.Currency}")}, tenant: {x.TenantSlug}")
            .ToList();

        if (!upcomingEvents.Any())
        {
            return new AiResponseDTO
            {
                Response = "No upcoming events found right now.",
                TokensUsed = 0
            };
        }

        var prompt = $"""
        You are an AI recommendation assistant for Eventix.

        Recommend 3 events for a buyer from this global upcoming event list.

        Events:
        {string.Join("\n", upcomingEvents)}

        Rules:
        - Recommend only events from the list.
        - Keep it short and friendly.
        - Explain briefly why each event is recommended.
        - Do not invent prices, speakers, or unavailable details.
        """;

        var response = await _ollamaClient.GenerateAsync(prompt, ct);

        return new AiResponseDTO
        {
            Response = response,
            TokensUsed = EstimateTokens(prompt + response)
        };
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

    private static string BuildDirectEventAnswer(
        List<Application.DTOs.Events.EventResponseDTO> events,
        string? requestedCategory,
        bool asksForFreeEvents)
    {
        var intro = "Here are the best matching upcoming events:";

        if (!string.IsNullOrWhiteSpace(requestedCategory))
        {
            intro = requestedCategory.Equals("Family", StringComparison.OrdinalIgnoreCase)
                ? "For families, these upcoming events are the best matches:"
                : $"For {requestedCategory.ToLowerInvariant()} events, these are good upcoming options:";
        }
        else if (asksForFreeEvents)
        {
            intro = "These upcoming events are free:";
        }

        var lines = events
            .Take(5)
            .Select((eventItem, index) =>
            {
                var price = eventItem.IsFree
                    ? "Free"
                    : $"Paid in {eventItem.Currency}";

                return $"{index + 1}. {eventItem.Title} - {eventItem.EventCategoryName}, {eventItem.VenueName}, starts {eventItem.StartUtc:yyyy-MM-dd HH:mm} UTC, {price}.";
            });

        return $"{intro}\n\n{string.Join("\n", lines)}";
    }

    private static string? DetectRequestedCategory(string prompt)
    {
        var text = prompt.ToLowerInvariant();

        var categorySynonyms = new Dictionary<string, string[]>
        {
            ["Music"] = ["music", "concert", "festival", "beats", "dj", "band"],
            ["Technology"] = ["technology", "tech", "ai", "software", "digital"],
            ["Business"] = ["business", "startup", "founder", "networking", "forum"],
            ["Sports"] = ["sports", "sport", "marathon", "fitness"],
            ["Education"] = ["education", "educational", "workshop", "masterclass", "learning"],
            ["Culture"] = ["culture", "cultural", "art", "jazz"],
            ["Food"] = ["food", "fair", "cuisine", "restaurant"],
            ["Theatre"] = ["theatre", "theater", "play", "premiere"],
            ["Charity"] = ["charity", "gala", "fundraiser"],
            ["Family"] = ["family", "families", "family-friendly", "kids", "children", "parents"]
        };

        foreach (var category in categorySynonyms)
        {
            if (category.Value.Any(keyword => text.Contains(keyword)))
                return category.Key;
        }

        return null;
    }

    private static string? DetectRequestedCity(string prompt)
    {
        var cities = new[]
        {
            "Prishtina",
            "Prizren",
            "Gjakova",
            "Peja",
            "Tirana"
        };

        return cities.FirstOrDefault(city =>
            prompt.Contains(city, StringComparison.OrdinalIgnoreCase));
    }

    private static (DateTime StartUtc, DateTime EndUtc)? DetectRequestedDateRange(string prompt)
    {
        var now = DateTime.UtcNow;
        var text = prompt.ToLowerInvariant();

        if (text.Contains("tomorrow"))
        {
            var start = now.Date.AddDays(1);
            return (start, start.AddDays(1));
        }

        if (text.Contains("today"))
        {
            var start = now.Date;
            return (start, start.AddDays(1));
        }

        var daysSinceMonday = ((int)now.DayOfWeek + 6) % 7;
        var thisWeekStart = now.Date.AddDays(-daysSinceMonday);

        if (text.Contains("next week"))
        {
            var nextWeekStart = thisWeekStart.AddDays(7);
            return (nextWeekStart, nextWeekStart.AddDays(7));
        }

        if (text.Contains("this week") || text.Contains("current week"))
        {
            return (thisWeekStart, thisWeekStart.AddDays(7));
        }

        return null;
    }

    private static bool IsComparisonRequest(string prompt)
    {
        return prompt.Contains("compare", StringComparison.OrdinalIgnoreCase) ||
               prompt.Contains("difference", StringComparison.OrdinalIgnoreCase) ||
               prompt.Contains("which is better", StringComparison.OrdinalIgnoreCase);
    }
}
