using LoanAPI.Models;
using LoanAPI.Repositories;
using LoanAPI.DTOs;

namespace LoanAPI.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _repository;
        private readonly UserRepository _userRepository;

        public CustomerService(CustomerRepository repository, UserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<List<CustomerResponseDto>> GetAllCustomers()
        {
            var customers = await _repository.GetAllCustomers();

            return customers.Select(c => new CustomerResponseDto
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                Salary = c.Salary,
                CIBILScore = c.CIBILScore,
                EmploymentType = c.EmploymentType,
                CreatedDate = c.CreatedDate
            }).ToList();
        }

        public async Task<CustomerResponseDto?> GetCustomerById(int id)
        {
            var customer = await _repository.GetCustomerById(id);
            if (customer == null)
                return null;

            return new CustomerResponseDto
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                Salary = customer.Salary,
                CIBILScore = customer.CIBILScore,
                EmploymentType = customer.EmploymentType,
                CreatedDate = customer.CreatedDate
            };
        }

        public async Task<(bool Success, string Message, int CustomerId)> RegisterCustomer(RegisterCustomerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return (false, "Full Name is required.", 0);

            int age = DateTime.Now.Year - dto.DateOfBirth.Year;
            if (dto.DateOfBirth > DateTime.Now.AddYears(-age))
                age--;

            if (age < 21)
                return (false, "Customer must be at least 21 years old.", 0);

            if (dto.Salary <= 0)
                return (false, "Salary must be greater than zero.", 0);

            if (dto.CIBILScore < 300 || dto.CIBILScore > 900)
                return (false, "Invalid CIBIL Score.", 0);

            var employment = dto.EmploymentType.ToLower();
            if (employment != "salaried" && employment != "self-employed" && employment != "business")
                return (false, "Invalid Employment Type.", 0);

            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return (false, "Username and Password are required.", 0);

            var customers = await _repository.GetAllCustomers();

            if (customers.Any(c => c.Email == dto.Email))
                return (false, "Email already exists.", 0);

            if (customers.Any(c => c.Phone == dto.Phone))
                return (false, "Phone number already exists.", 0);

            var existingUser = await _userRepository.GetUserByUsername(dto.Username);
            if (existingUser != null)
                return (false, "Username already exists.", 0);

            var customer = new Customer
            {
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email,
                Phone = dto.Phone,
                Salary = dto.Salary,
                CIBILScore = dto.CIBILScore,
                EmploymentType = dto.EmploymentType,
                CreatedDate = DateTime.Now
            };

            await _repository.AddCustomer(customer);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Customer",
                CustomerId = customer.CustomerId
            };

            await _userRepository.AddUser(user);

            return (true, "Customer registered successfully.", customer.CustomerId);
        }

        public async Task<(bool Success, string Message)> UpdateCustomer(int id, UpdateCustomerDto dto)
        {
            var customer = await _repository.GetCustomerById(id);
            if (customer == null)
                return (false, "Customer not found.");

            if (string.IsNullOrWhiteSpace(dto.FullName))
                return (false, "Full Name is required.");

            if (dto.Salary <= 0)
                return (false, "Salary must be greater than zero.");

            if (dto.CIBILScore < 300 || dto.CIBILScore > 900)
                return (false, "Invalid CIBIL Score.");

            var employment = dto.EmploymentType.ToLower();
            if (employment != "salaried" && employment != "self-employed" && employment != "business")
                return (false, "Invalid Employment Type.");

            // only re-check uniqueness if the value actually changed
            var allCustomers = await _repository.GetAllCustomers();
            if (dto.Email != customer.Email && allCustomers.Any(c => c.Email == dto.Email))
                return (false, "Email already exists.");
            if (dto.Phone != customer.Phone && allCustomers.Any(c => c.Phone == dto.Phone))
                return (false, "Phone number already exists.");

            customer.FullName = dto.FullName;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.Salary = dto.Salary;
            customer.CIBILScore = dto.CIBILScore;
            customer.EmploymentType = dto.EmploymentType;

            await _repository.UpdateCustomer(customer);
            return (true, "Customer updated successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteCustomer(int id)
        {
            var customer = await _repository.GetCustomerById(id);
            if (customer == null)
                return (false, "Customer not found.");

            await _repository.DeleteCustomer(customer);
            return (true, "Customer deleted successfully.");
        }
    }
}