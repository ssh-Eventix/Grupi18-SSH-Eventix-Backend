using Eventix.Application.DTOs.Payment;
using Eventix.Application.DTOs.PaymentMethod;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Services
{
    public interface IPaymentMethodService
    {
        Task<PaymentMethodDto> GetByIdAsync(Guid id);

        Task<List<PaymentMethodDto>> GetAllAsync();

        Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto request);

        Task<PaymentMethodDto> UpdateAsync(Guid id, UpdatePaymentMethodDto request);

        Task DeleteAsync(Guid id);

        Task ActivateAsync(Guid id);

        Task DeactivateAsync(Guid id);
    }
}