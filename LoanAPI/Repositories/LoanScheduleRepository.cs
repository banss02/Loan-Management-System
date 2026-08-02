using Microsoft.EntityFrameworkCore;
using LoanAPI.Data;
using LoanAPI.Models;

namespace LoanAPI.Repositories
{
    public class LoanScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public LoanScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LoanSchedule>> GetByLoanId(int loanId) =>
            await _context.LoanSchedules
                .Where(s => s.LoanId == loanId)
                .OrderBy(s => s.InstallmentNo)
                .ToListAsync();

        public async Task<LoanSchedule?> GetById(int scheduleId) =>
            await _context.LoanSchedules.FindAsync(scheduleId);

        public async Task AddRange(List<LoanSchedule> schedules)
        {
            _context.LoanSchedules.AddRange(schedules);
            await _context.SaveChangesAsync();
        }

        public async Task Update(LoanSchedule schedule)
        {
            _context.LoanSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }
    }
}
