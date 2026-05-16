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
    public class BookingRepository : IBookingRepository
    {
        private readonly TenantDbContext _context;

        public BookingRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => !b.IsDeleted)
                .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Tickets)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Where(b => !b.IsDeleted)
                .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Tickets)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId && !b.IsDeleted)
                .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Tickets)
                .ToListAsync();
        }

        public async Task<Booking?> GetWithItemsAsync(Guid id)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => !b.IsDeleted)
                .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Tickets)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
