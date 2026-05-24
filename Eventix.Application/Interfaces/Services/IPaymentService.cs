using Eventix.Application.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentDto> GetByIdAsync(Guid id);

        Task<List<PaymentDto>> GetAllAsync();

        Task<List<PaymentDto>> GetByBookingIdAsync(Guid bookingId);

        Task<PaymentDto> CreatePayment(CreatePaymentDto request);
    }
}