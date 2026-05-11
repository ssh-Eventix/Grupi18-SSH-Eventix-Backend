using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Services
{
    public interface ITicketTypeService
    {
        Task<TicketType?> GetByIdAsync(Guid id);

        Task<List<TicketType>> GetByEventIdAsync(Guid eventId);

        Task<List<TicketType>> GetAvailableByEventIdAsync(Guid eventId);

        Task<bool> IsAvailableAsync(Guid ticketTypeId, int quantity);

        Task ReduceStockAsync(Guid ticketTypeId, int quantity);
    }
}
