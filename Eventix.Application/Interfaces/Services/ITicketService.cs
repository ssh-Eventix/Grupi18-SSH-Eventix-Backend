using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
