using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid id);
        Task<List<Booking>> GetByUserIdAsync(Guid userId);
        Task<Booking?> GetWithItemsAsync(Guid id);

        Task AddAsync(Booking booking);
        Task SaveChangesAsync();
    }
}
