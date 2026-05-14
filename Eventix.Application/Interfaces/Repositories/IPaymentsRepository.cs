using Eventix.Domain.Entities;
using Eventix.Domain.Enums;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllAsync();

        Task<Payment?> GetByIdAsync(Guid id);

        Task<List<Payment>> GetByBookingIdAsync(Guid bookingId);

        Task<List<Payment>> GetByStatusAsync(PaymentStatus status);

        Task AddAsync(Payment payment);

        void Update(Payment payment);

        void Delete(Payment payment);

        Task SaveChangesAsync();
    }
}