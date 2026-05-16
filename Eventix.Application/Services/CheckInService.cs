using Eventix.Application.DTOs.CheckIns;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class CheckInService : ICheckInService
{
    private readonly ICheckInRepository _repo;
    private readonly ITenantContext _tenant;

    public CheckInService(ICheckInRepository repo, ITenantContext tenant)
    {
        _repo = repo;
        _tenant = tenant;
    }

    public async Task<List<CheckInDto>> GetAllAsync(CancellationToken ct)
    {
        var data = await _repo.GetAllAsync(ct);
        return data.Select(Map).ToList();
    }

    public async Task<CheckInDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var data = await _repo.GetByIdAsync(id, ct);
        return data is null ? null : Map(data);
    }

    public async Task<CheckInDto> CreateAsync(CreateCheckInDTO dto, CancellationToken ct)
    {
        var exists = await _repo.GetByTicketIdAsync(dto.TicketId, ct);

        if (exists != null)
            throw new InvalidOperationException("Ticket already checked in");

        var entity = new CheckIn
        {
            Id = Guid.NewGuid(),
            TenantId = _tenant.TenantId,
            TicketId = dto.TicketId,
            CheckedInByUserId = dto.CheckedInByUserId,
            Notes = dto.Notes,
            CheckInTime = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return Map(entity);
    }

    private static CheckInDto Map(CheckIn x) => new()
    {
        Id = x.Id,
        TicketId = x.TicketId,
        CheckedInByUserId = x.CheckedInByUserId,
        CheckInTime = x.CheckInTime,
        Notes = x.Notes
    };
}