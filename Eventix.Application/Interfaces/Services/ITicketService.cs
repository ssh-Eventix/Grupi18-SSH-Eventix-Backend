using Eventix.Application.DTOs.Ticket;
using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<List<Ticket>> GenerateTicketsAsync(Guid bookingItemId, int quantity);
        Task<List<TicketDto>> GetAllAsync();
        Task<Ticket?> GetByIdAsync(Guid id);
        Task<TicketDto?> GetDtoByIdAsync(Guid id);
        Task<Ticket?> GetByCodeAsync(string ticketCode);
        Task<TicketDto?> GetDtoByCodeAsync(string ticketCode);
        Task<bool> ValidateTicketAsync(string ticketCode);
        Task CheckInAsync(string ticketCode);
    }
}
