using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly TenantDbContext _context;

        public PaymentRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .AsNoTracking()
                .Include(p => p.Booking)
                .Include(p => p.PaymentMethod)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Payment>> GetByBookingIdAsync(Guid bookingId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.BookingId == bookingId)
                .Include(p => p.PaymentMethod)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.Status == status)
                .Include(p => p.Booking)
                .Include(p => p.PaymentMethod)
                .ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public void Update(Payment payment)
        {
            _context.Payments.Update(payment);
        }

        public void Delete(Payment payment)
        {
            _context.Payments.Remove(payment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}