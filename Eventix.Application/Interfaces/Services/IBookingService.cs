using Eventix.Application.DTOs.Booking;

namespace Eventix.Application.Interfaces.Services
{
    public interface IBookingService
    {
        Task<List<BookingDto>> GetAllAsync();
        Task<BookingDto> GetByIdAsync(Guid id);
        Task<List<BookingDto>> GetUserBookings(Guid userId);
        Task<BookingDto> CreateBooking(CreateBookingRequest request);
        Task<bool> UpdateBookingStatus(Guid id, UpdateBookingStatusRequest request);
        Task<bool> DeleteBooking(Guid id);
    }
}
