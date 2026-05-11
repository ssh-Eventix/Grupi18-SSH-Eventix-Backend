using Eventix.Application.DTOs.Booking;

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
