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
    public class TicketRepository : ITicketRepository
    {
        private readonly TenantDbContext _context;

        public TicketRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            return await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Ticket?> GetByCodeAsync(string ticketCode)
        {
            return await _context.Tickets
                .FirstOrDefaultAsync(t => t.TicketCode == ticketCode);
        }

        public async Task<List<Ticket>> GetByBookingItemIdAsync(Guid bookingItemId)
        {
            return await _context.Tickets
                .Where(t => t.BookingItemId == bookingItemId)
                .ToListAsync();
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
        }

        public async Task AddRangeAsync(List<Ticket> tickets)
        {
            await _context.Tickets.AddRangeAsync(tickets);
        }

        public void Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
