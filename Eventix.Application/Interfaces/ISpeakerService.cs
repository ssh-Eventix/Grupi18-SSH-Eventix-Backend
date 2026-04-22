using Eventix.Application.DTOs.Speakers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces;

public interface ISpeakerService
{
    Task<List<SpeakerDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SpeakerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SpeakerDto> CreateAsync(CreateSpeakerDto dto, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, UpdateSpeakerDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}