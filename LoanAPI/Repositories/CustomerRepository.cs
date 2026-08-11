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

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.CustomerId == customer.CustomerId); 

    if (user != null)
    {
        _context.Users.Remove(user);
    }

    var documents = _context.Documents
        .Where(d => d.CustomerId == customer.CustomerId);
    _context.Documents.RemoveRange(documents);

    var loans = await _context.Loans
        .Where(l => l.CustomerId == customer.CustomerId)
        .ToListAsync();

    foreach (var loan in loans)
    {
        var schedules = _context.LoanSchedules
            .Where(s => s.LoanId == loan.LoanId);
        _context.LoanSchedules.RemoveRange(schedules);

        var payments = _context.Payments
            .Where(p => p.LoanId == loan.LoanId);
        _context.Payments.RemoveRange(payments);
    }

    _context.Loans.RemoveRange(loans);

    _context.Customers.Remove(customer);

    await _context.SaveChangesAsync();
}

    }
}