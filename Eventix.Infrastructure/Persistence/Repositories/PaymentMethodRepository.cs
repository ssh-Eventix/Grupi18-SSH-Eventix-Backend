using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly PublicDbContext _context;

        public PaymentMethodRepository(PublicDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentMethod>> GetAllAsync()
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .Where(pm => !pm.IsDeleted)
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetByIdAsync(Guid id)
        {
            return await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == id && !pm.IsDeleted);
        }

        public async Task<List<PaymentMethod>> GetActiveAsync()
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .Where(pm => pm.IsActive && !pm.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<PaymentMethod>> GetByProviderAsync(PaymentProvider provider)
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .Where(pm => pm.Provider == provider && !pm.IsDeleted)
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetWithPaymentsAsync(Guid id)
        {
            return await _context.PaymentMethods
                .Include(pm => pm.Payments)
                .FirstOrDefaultAsync(pm => pm.Id == id && !pm.IsDeleted);
        }

        public async Task AddAsync(PaymentMethod paymentMethod)
        {
            paymentMethod.TenantId = Guid.Empty;
            await _context.PaymentMethods.AddAsync(paymentMethod);
        }

        public void Update(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Update(paymentMethod);
        }

        public void Delete(PaymentMethod paymentMethod)
        {
            paymentMethod.IsDeleted = true;
            paymentMethod.UpdatedAtUtc = DateTime.UtcNow;
            _context.PaymentMethods.Update(paymentMethod);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
