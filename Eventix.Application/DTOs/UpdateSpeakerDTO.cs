namespace Eventix.Application.DTOs.Speakers;

public class UpdateSpeakerDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }

    public string? Email { get; set; }
    public string? Phone { get; set; }

    public string? ProfileImageUrl { get; set; }
}