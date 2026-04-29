using System;

namespace Eventix.Application.DTOs.Speaker;

public class SpeakerDto
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }

    public string? Email { get; set; }
    public string? Phone { get; set; }

    public string? ProfileImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}