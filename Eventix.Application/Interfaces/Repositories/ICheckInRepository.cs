using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface ICheckInRepository
{
    Task<List<CheckIn>> GetAllAsync(Guid tenantId, CancellationToken ct);
    Task<CheckIn?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct);
    Task<CheckIn?> GetByTicketIdAsync(Guid ticketId, Guid tenantId, CancellationToken ct);

    Task AddAsync(CheckIn entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}