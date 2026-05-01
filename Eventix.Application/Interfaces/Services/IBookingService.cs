using Eventix.Application.DTOs.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Services
{
    public interface IBookingService
    {
        Task<List<BookingDto>> GetAllAsync();
        Task<BookingDto> GetByIdAsync(Guid id);
        Task<List<BookingDto>> GetUserBookings(Guid userId);
        Task<BookingDto> CreateBooking(CreateBookingRequest request);
    }
}
