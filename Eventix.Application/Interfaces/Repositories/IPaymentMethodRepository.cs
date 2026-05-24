using Eventix.Domain.Entities;
using Eventix.Domain.Enums;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface IPaymentMethodRepository
    {
        Task<List<PaymentMethod>> GetAllAsync();

        Task<PaymentMethod?> GetByIdAsync(Guid id);

        Task<List<PaymentMethod>> GetActiveAsync();

        Task<List<PaymentMethod>> GetByProviderAsync(PaymentProvider provider);

        Task<PaymentMethod?> GetWithPaymentsAsync(Guid id);

        Task AddAsync(PaymentMethod paymentMethod);

        void Update(PaymentMethod paymentMethod);

        void Delete(PaymentMethod paymentMethod);

        Task SaveChangesAsync();
    }
}