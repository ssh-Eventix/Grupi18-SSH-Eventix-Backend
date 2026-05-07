using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Infrastructure.Persistence.Repositories
{
    public class TicketTypeRepository : ITicketTypeRepository
    {
        private readonly TenantDbContext _context;

        public TicketTypeRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<TicketType?> GetByIdAsync(Guid id)
        {
            return await _context.TicketTypes
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<TicketType>> GetByEventIdAsync(Guid eventId)
        {
            return await _context.TicketTypes
                .AsNoTracking()
                .Where(t => t.EventId == eventId)
                .ToListAsync();
        }

        public async Task<List<TicketType>> GetAvailableByEventIdAsync(Guid eventId, DateTime now)
        {
            return await _context.TicketTypes
                .AsNoTracking()
                .Where(t =>
                    t.EventId == eventId &&
                    t.QuantityAvailable > 0 &&
                    t.SaleStartDate <= now &&
                    t.SaleEndDate >= now)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.TicketTypes.AnyAsync(t => t.Id == id);
        }

        public async Task AddAsync(TicketType ticketType)
        {
            await _context.TicketTypes.AddAsync(ticketType);
        }

        public void Update(TicketType ticketType)
        {
            _context.TicketTypes.Update(ticketType);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
