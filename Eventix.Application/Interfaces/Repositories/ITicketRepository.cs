using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task<List<Ticket>> GetAllAsync();

        Task<Ticket?> GetByIdAsync(Guid id);

        Task<Ticket?> GetByCodeAsync(string ticketCode);

        Task<List<Ticket>> GetByBookingItemIdAsync(Guid bookingItemId);

        Task AddAsync(Ticket ticket);

        Task AddRangeAsync(List<Ticket> tickets);

        void Update(Ticket ticket);

        Task SaveChangesAsync();
    }
}
