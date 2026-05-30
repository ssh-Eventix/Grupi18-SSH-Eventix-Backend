using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly TenantDbContext _context;

        public PaymentMethodRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentMethod>> GetAllAsync()
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetByIdAsync(Guid id)
        {
            return await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == id);
        }

        public async Task<List<PaymentMethod>> GetActiveAsync()
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .Where(pm => pm.IsActive)
                .ToListAsync();
        }

        public async Task<List<PaymentMethod>> GetByProviderAsync(PaymentProvider provider)
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .Where(pm => pm.Provider == provider)
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetWithPaymentsAsync(Guid id)
        {
            return await _context.PaymentMethods
                .Include(pm => pm.Payments)
                .FirstOrDefaultAsync(pm => pm.Id == id);
        }

        public async Task AddAsync(PaymentMethod paymentMethod)
        {
            await _context.PaymentMethods.AddAsync(paymentMethod);
        }

        public void Update(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Update(paymentMethod);
        }

        public void Delete(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Remove(paymentMethod);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}