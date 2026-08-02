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
            return payments.Select(ToDto).ToList();
        }

        public async Task<List<PaymentResponseDto>> GetPaymentsByCustomerId(int customerId)
        {
            var payments = await _paymentRepository.GetPaymentsByCustomerId(customerId);
            return payments.Select(ToDto).ToList();
        }

        public async Task<(bool Success, string Message)> MakePayment(PaymentDto dto)
        {
            if (dto.Amount <= 0)
                return (false, "Amount must be greater than zero.");

            // If this payment is tied to a specific EMI installment, validate against it first
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

            // Re-check: only mark the installment paid once total payments against it
            // reach (or exceed) the EMI amount - partial payments accumulate instead of
            // instantly flipping IsPaid on the first rupee.
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

        private static PaymentResponseDto ToDto(Payment p) => new PaymentResponseDto
        {
            PaymentId = p.PaymentId,
            LoanId = p.LoanId,
            ScheduleId = p.ScheduleId,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate
        };
    }
}