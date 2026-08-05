using Microsoft.EntityFrameworkCore;
using LoanAPI.Data;
using LoanAPI.Models;

namespace LoanAPI.Repositories
{
    public class CustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllCustomers() =>
            await _context.Customers.ToListAsync();

        public async Task<Customer?> GetCustomerById(int id) =>
            await _context.Customers.FindAsync(id);

        public async Task AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomer(Customer customer)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        // Which admin manages this one customer (used for a single ownership check)
        public async Task<int?> GetAssignedAdminId(int customerId) =>
            (await _context.Customers.FindAsync(customerId))?.AssignedAdminId;

        public async Task<List<int>> GetCustomerIdsAssignedToAdmin(int adminUserId) =>
            await _context.Customers
                .Where(c => c.AssignedAdminId == adminUserId)
                .Select(c => c.CustomerId)
                .ToListAsync();
    }
}