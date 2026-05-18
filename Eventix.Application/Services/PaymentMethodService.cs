using Eventix.Application.DTOs.PaymentMethod;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        public readonly IPaymentMethodRepository _paymentMethodRepository;
        public readonly IPaymentRepository _paymentRepository;

        public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, IPaymentRepository paymentRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentMethodDto> GetByIdAsync(Guid id)
        {
            var method = await _paymentMethodRepository.GetByIdAsync(id);

            if (method == null)
                throw new Exception("Payment method not found");

            return Map(method);
        }

        public async Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto request)
        {
            var method = new PaymentMethod
            {
                Name = request.Name,
                Description = request.Description,
                Provider = request.Provider,
                IsActive = true
            };

            await _paymentMethodRepository.AddAsync(method);
            await _paymentMethodRepository.SaveChangesAsync();

            return Map(method);
        }

        // READ ALL
        public async Task<List<PaymentMethodDto>> GetAllAsync()
        {
            var methods = await _paymentMethodRepository.GetAllAsync();
            return methods.Select(Map).ToList();
        }

        public async Task ActivateAsync(Guid id)
        {
            var method = await _paymentMethodRepository.GetByIdAsync(id);

            if (method == null)
                throw new Exception("Not found");

            method.IsActive = true;

            _paymentMethodRepository.Update(method);
            await _paymentMethodRepository.SaveChangesAsync();
        }

        public async Task DeactivateAsync(Guid id)
        {
            var method = await _paymentMethodRepository.GetByIdAsync(id);

            if (method == null)
                throw new Exception("Not found");

            method.IsActive = false;

            _paymentMethodRepository.Update(method);
            await _paymentMethodRepository.SaveChangesAsync();
        }
        private static PaymentMethodDto Map(PaymentMethod method)
        {
            return new PaymentMethodDto
            {
                Id = method.Id,
                Name = method.Name,
                Description = method.Description,
                Provider = method.Provider.ToString(),
                IsActive = method.IsActive
            };
        }
    }
}
