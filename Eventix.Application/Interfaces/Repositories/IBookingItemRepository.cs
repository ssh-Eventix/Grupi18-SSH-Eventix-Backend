using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface IBookingItemRepository
    {
        Task AddAsync(BookingItem item);
    }
}
