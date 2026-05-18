using Eventix.Application.DTOs.Booking;
using Eventix.Application.DTOs.Payment;
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
    public class PaymentService : IPaymentService
    {
        public readonly IPaymentRepository _paymentRepository;
        public readonly IBookingRepository _bookingRepository;

        public PaymentService(IPaymentRepository paymentRepository, IBookingRepository bookingRepository)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
        }

        //We use repositories only on services, and use services only for frontend communication
        //thats why we have same functions as repository here, bvut there we validate them and convert to DTO
        public async Task<PaymentDto> GetByIdAsync(Guid id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                throw new Exception("Payment not found");
            return MapPayment(payment);
        }

        public async Task<List<PaymentDto>> GetAllAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return MapPayments(payments);
        }

        public async Task<List<PaymentDto>> GetByBookingIdAsync(Guid bookingId)
        {
            var payments = await _paymentRepository.GetByBookingIdAsync(bookingId);
            return MapPayments(payments);
        }

        public async Task<PaymentDto> CreatePayment(CreatePaymentDto request)
        {
            if (request.BookingId == null)
            {
                throw new Exception("BookingId is required");
            }

            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking == null)
                throw new Exception("Booking not found");

            var payment = new Payment
            {
                BookingId = request.BookingId,
                Amount = request.Amount,
                PaymentMethodId = request.PaymentMethodId,
                Status = request.Status,
                PaidAt = request.PaidAt
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return MapPayment(payment);
        }

        private PaymentDto MapPayment(Payment payment) //We use mapping to convert entity to DTO becasue Entity is only for
                                                       //DB communication its not safe to use for frontend
        {
            return new PaymentDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentMethodId = payment.PaymentMethodId,
                Status = payment.Status.ToString(),
                PaidAt = payment.PaidAt
            };
        }

        private static List<PaymentDto> MapPayments(List<Payment> payments)
        {
            return payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                BookingId = p.BookingId,
                Amount = p.Amount,
                PaymentMethodId = p.PaymentMethodId,
                Status = p.Status.ToString(), // konverton enum ne string (better for frontend)
                PaidAt = p.PaidAt
            }).ToList();
        }
    }
}
