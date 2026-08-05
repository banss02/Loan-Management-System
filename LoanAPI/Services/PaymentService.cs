using LoanAPI.Models;
using LoanAPI.Repositories;
using LoanAPI.DTOs;

namespace LoanAPI.Services
{
    public class PaymentService
    {
        private readonly PaymentRepository _paymentRepository;
        private readonly LoanScheduleRepository _scheduleRepository;

        public PaymentService(PaymentRepository paymentRepository, LoanScheduleRepository scheduleRepository)
        {
            _paymentRepository = paymentRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<List<PaymentResponseDto>> GetAllPayments()
        {
            var payments = await _paymentRepository.GetAllPayments();
            return payments.Select(p => ToDto(p, p.Loan?.CustomerId ?? 0)).ToList();
        }

        public async Task<List<PaymentResponseDto>> GetPaymentsByCustomerId(int customerId)
        {
            var payments = await _paymentRepository.GetPaymentsByCustomerId(customerId);
            return payments.Select(p => ToDto(p, customerId)).ToList();
        }

        public async Task<(bool Success, string Message)> MakePayment(PaymentDto dto)
        {
            if (dto.Amount <= 0)
                return (false, "Amount must be greater than zero.");

            if (dto.ScheduleId.HasValue)
            {
                var schedule = await _scheduleRepository.GetById(dto.ScheduleId.Value);
                if (schedule == null)
                    return (false, "EMI installment not found.");

                if (schedule.IsPaid)
                    return (false, "This installment is already fully paid.");
            }

            var payment = new Payment
            {
                LoanId = dto.LoanId,
                ScheduleId = dto.ScheduleId,
                Amount = dto.Amount,
                PaymentDate = DateTime.Now
            };

            await _paymentRepository.AddPayment(payment);

            if (dto.ScheduleId.HasValue)
            {
                var schedule = await _scheduleRepository.GetById(dto.ScheduleId.Value);
                if (schedule != null)
                {
                    var totalPaid = await _paymentRepository.GetTotalPaidForSchedule(dto.ScheduleId.Value);

                    if (totalPaid >= schedule.EMIAmount)
                    {
                        schedule.IsPaid = true;
                        await _scheduleRepository.Update(schedule);
                    }
                }
            }

            return (true, "Payment recorded successfully.");
        }

        private static PaymentResponseDto ToDto(Payment p, int customerId) => new PaymentResponseDto
        {
            PaymentId = p.PaymentId,
            LoanId = p.LoanId,
            CustomerId = customerId,
            ScheduleId = p.ScheduleId,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate
        };
    }
}