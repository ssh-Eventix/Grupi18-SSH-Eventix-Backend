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
    public class BookingItemRepository : IBookingItemRepository
    {
        private readonly TenantDbContext _context;

        public BookingItemRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BookingItem item)
        {
            await _context.BookingItems.AddAsync(item);
        }
    }
}
