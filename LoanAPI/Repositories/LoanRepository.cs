using Microsoft.EntityFrameworkCore;
using LoanAPI.Data;
using LoanAPI.Models;

namespace LoanAPI.Repositories
{
    public class LoanRepository
    {
        private readonly ApplicationDbContext _context;

        public LoanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Loan>> GetAllLoans() =>
            await _context.Loans.ToListAsync();

        public async Task<List<Loan>> GetLoansByCustomerId(int customerId) =>
            await _context.Loans.Where(l => l.CustomerId == customerId).ToListAsync();

        public async Task<Loan?> GetLoanById(int id) =>
            await _context.Loans.FindAsync(id);

        public async Task AddLoan(Loan loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLoan(Loan loan)
        {
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();
        }
    }
}
