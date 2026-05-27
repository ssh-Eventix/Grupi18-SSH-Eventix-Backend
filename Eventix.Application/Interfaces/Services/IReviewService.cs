using Eventix.Application.DTOs.Review;

namespace Eventix.Application.Interfaces.Services;

public interface IReviewService
{
    Task<List<ReviewDto>> GetAllAsync(CancellationToken ct);
    Task<ReviewDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<ReviewDto> CreateAsync(CreateReviewDTO dto, CancellationToken ct);

    Task<List<ReviewDto>> GetByEventIdAsync(Guid eventId, CancellationToken ct = default);
    Task<List<ReviewDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}