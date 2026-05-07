using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface ITicketTypeRepository
    {
        Task<TicketType?> GetByIdAsync(Guid id);
        Task<List<TicketType>> GetByEventIdAsync(Guid eventId);
        Task<List<TicketType>> GetAvailableByEventIdAsync(Guid eventId, DateTime now);
        Task<bool> ExistsAsync(Guid id);
        Task AddAsync(TicketType ticketType);
        void Update(TicketType ticketType);
        Task SaveChangesAsync();
    }
}
