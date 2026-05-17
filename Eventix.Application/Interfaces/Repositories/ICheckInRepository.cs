using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface ICheckInRepository
{
    Task<List<CheckIn>> GetAllAsync(CancellationToken ct = default);
    Task<CheckIn?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CheckIn?> GetByTicketIdAsync(Guid ticketId, CancellationToken ct = default);

    Task AddAsync(CheckIn entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}