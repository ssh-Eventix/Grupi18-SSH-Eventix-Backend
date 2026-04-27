using Eventix.Application.DTOs.Speaker;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eventix.Application.Services;

public class SpeakerService : ISpeakerService
{
    private readonly ISpeakerRepository _speakerRepository;

    public SpeakerService(ISpeakerRepository speakerRepository)
    {
        _speakerRepository = speakerRepository;
    }

    public async Task<List<SpeakerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var speakers = await _speakerRepository.GetAllAsync(cancellationToken);
        return speakers.Select(MapToDto).ToList();
    }

    public async Task<SpeakerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var speaker = await _speakerRepository.GetByIdAsync(id, cancellationToken);
        return speaker is null ? null : MapToDto(speaker);
    }

    public async Task<SpeakerDto> CreateAsync(CreateSpeakerDto dto, CancellationToken cancellationToken = default)
    {
        var speaker = new Speaker
        {
            Id = Guid.NewGuid(),
            TenantId = dto.TenantId,
            FullName = dto.FullName,
            Bio = dto.Bio,
            Email = dto.Email,
            Phone = dto.Phone,
            ProfileImageUrl = dto.ProfileImageUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _speakerRepository.AddAsync(speaker, cancellationToken);
        await _speakerRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(speaker);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateSpeakerDto dto, CancellationToken cancellationToken = default)
    {
        var speaker = await _speakerRepository.GetByIdAsync(id, cancellationToken);

        if (speaker is null)
            return false;

        speaker.FullName = dto.FullName;
        speaker.Bio = dto.Bio;
        speaker.Email = dto.Email;
        speaker.Phone = dto.Phone;
        speaker.ProfileImageUrl = dto.ProfileImageUrl;

        _speakerRepository.Update(speaker);
        await _speakerRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var speaker = await _speakerRepository.GetByIdAsync(id, cancellationToken);

        if (speaker is null)
            return false;

        _speakerRepository.Delete(speaker);
        await _speakerRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static SpeakerDto MapToDto(Speaker speaker)
    {
        return new SpeakerDto
        {
            Id = speaker.Id,
            TenantId = speaker.TenantId,
            FullName = speaker.FullName,
            Bio = speaker.Bio,
            Email = speaker.Email,
            Phone = speaker.Phone,
            ProfileImageUrl = speaker.ProfileImageUrl,
            CreatedAt = speaker.CreatedAt
        };
    }
}