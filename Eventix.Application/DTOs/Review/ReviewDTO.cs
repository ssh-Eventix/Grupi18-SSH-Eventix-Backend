namespace Eventix.Application.DTOs.Review;
using System;
public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}