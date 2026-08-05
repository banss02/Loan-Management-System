using Microsoft.EntityFrameworkCore;
using LoanAPI.Data;
using LoanAPI.Models;

namespace LoanAPI.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsername(string username) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetUserById(int userId) =>
            await _context.Users.FindAsync(userId);

        public async Task AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAdminExists() =>
            await _context.Users.AnyAsync(u => u.Role == "Admin");

        public async Task<List<User>> GetAllAdmins() =>
            await _context.Users
                .Where(u => u.Role == "Admin")
                .OrderBy(u => u.UserId)
                .ToListAsync();
    }
}