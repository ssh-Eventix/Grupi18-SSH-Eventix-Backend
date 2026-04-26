using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TenantDbContext _context;

        public TicketRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<Ticket> tickets)
        {
            await _context.Tickets.AddRangeAsync(tickets);
        }
    }

}
