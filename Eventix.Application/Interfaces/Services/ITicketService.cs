using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<List<Ticket>> GenerateTicketsAsync(Guid bookingItemId, int quantity);
        Task<Ticket?> GetByIdAsync(Guid id);
        Task<Ticket?> GetByCodeAsync(string ticketCode);
        Task<bool> ValidateTicketAsync(string ticketCode);
        Task CheckInAsync(string ticketCode);
    }
}
