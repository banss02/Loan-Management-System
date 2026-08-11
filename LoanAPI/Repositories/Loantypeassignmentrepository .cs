using Microsoft.EntityFrameworkCore;
using LoanAPI.Data;
using LoanAPI.Models;

namespace LoanAPI.Repositories
{
    public class LoanTypeAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public LoanTypeAssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LoanTypeAssignment>> GetAll() =>
            await _context.LoanTypeAssignments.ToListAsync();

        public async Task<int?> GetAdminIdForLoanType(string loanType)
        {
            var assignment = await _context.LoanTypeAssignments
                .FirstOrDefaultAsync(a => a.LoanType.ToLower() == loanType.ToLower());
            return assignment?.UserId;
        }

        public async Task<List<string>> GetLoanTypesForAdmin(int adminId) =>
            await _context.LoanTypeAssignments
                .Where(a => a.UserId == adminId)
                .Select(a => a.LoanType)
                .ToListAsync();
        


        public async Task AddOrUpdate(string loanType, int adminId)
        {
            var existing = await _context.LoanTypeAssignments
                .FirstOrDefaultAsync(a => a.LoanType == loanType);

            if (existing != null)
            {
                existing.UserId = adminId;
            }
            else
            {
                _context.LoanTypeAssignments.Add(new LoanTypeAssignment
                {
                    LoanType = loanType,
                    UserId  = adminId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<LoanTypeAssignment?> GetByAdminId(int adminId)
{
    return await _context.LoanTypeAssignments
        .FirstOrDefaultAsync(x => x.UserId == adminId);
}
    }
}