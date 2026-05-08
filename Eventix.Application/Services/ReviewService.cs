using Eventix.Application.DTOs.Review;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using System;

namespace Eventix.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repo;
    private readonly ITenantContext _tenant;

    public ReviewService(IReviewRepository repo, ITenantContext tenant)
    {
        _repo = repo;
        _tenant = tenant;
    }

    public async Task<List<ReviewDto>> GetAllAsync(CancellationToken ct)
    {
        var data = await _repo.GetAllAsync(_tenant.TenantId, ct);
        return data.Select(Map).ToList();
    }

    public async Task<ReviewDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var data = await _repo.GetByIdAsync(id, _tenant.TenantId, ct);
        return data is null ? null : Map(data);
    }

    public async Task<ReviewDto> CreateAsync(CreateReviewDTO dto, CancellationToken ct)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new Exception("Rating must be between 1 and 5");

        var entity = new Review
        {
            Id = Guid.NewGuid(),
            TenantId = _tenant.TenantId,
            EventId = dto.EventId,
            UserId = dto.UserId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return Map(entity);
    }

    private static ReviewDto Map(Review x) => new()
    {
        Id = x.Id,
        EventId = x.EventId,
        UserId = x.UserId,
        Rating = x.Rating,
        Comment = x.Comment,
        CreatedAt = x.CreatedAt
    };
}