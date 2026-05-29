using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Application.Interfaces.Common;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Eventix.Domain.Enums;

namespace Eventix.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly TenantDbContext _context;
        private readonly ITenantContext _tenantContext;

        public PaymentRepository(TenantDbContext context, ITenantContext tenantContext)
        {
            _context = context;
            _tenantContext = tenantContext;
        }

        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.TenantId == _tenantContext.TenantId && !p.IsDeleted)
                .Include(p => p.Booking)
                .Include(p => p.PaymentMethod)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == _tenantContext.TenantId && !p.IsDeleted);
        }

        public async Task<List<Payment>> GetByBookingIdAsync(Guid bookingId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.BookingId == bookingId && p.TenantId == _tenantContext.TenantId && !p.IsDeleted)
                .Include(p => p.PaymentMethod)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.Status == status && p.TenantId == _tenantContext.TenantId && !p.IsDeleted)
                .Include(p => p.Booking)
                .Include(p => p.PaymentMethod)
                .ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            payment.TenantId = _tenantContext.TenantId;
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
