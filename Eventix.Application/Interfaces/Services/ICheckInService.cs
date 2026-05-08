using Eventix.Application.DTOs.CheckIns;

namespace Eventix.Application.Interfaces.Services;

public interface ICheckInService
{
    Task<List<CheckInDto>> GetAllAsync(CancellationToken ct);
    Task<CheckInDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<CheckInDto> CreateAsync(CreateCheckInDTO dto, CancellationToken ct);
}