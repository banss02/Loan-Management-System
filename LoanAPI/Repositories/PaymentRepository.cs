using Microsoft.EntityFrameworkCore;
using LoanAPI.Data;
using LoanAPI.Models;

namespace LoanAPI.Repositories
{
    public class PaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetAllPayments() =>
            await _context.Payments.ToListAsync();

        public async Task<List<Payment>> GetPaymentsByCustomerId(int customerId) =>
            await _context.Payments
                .Join(_context.Loans,
                    p => p.LoanId,
                    l => l.LoanId,
                    (p, l) => new { Payment = p, l.CustomerId })
                .Where(x => x.CustomerId == customerId)
                .Select(x => x.Payment)
                .ToListAsync();

        public async Task AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalPaidForSchedule(int scheduleId) =>
            await _context.Payments
                .Where(p => p.ScheduleId == scheduleId)
                .SumAsync(p => p.Amount);
    }
}