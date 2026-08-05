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

        public async Task<List<AdminCustomerResponseDto>> GetAllCustomersForAdmin()
        {
            var customers = await _repository.GetAllCustomers();
            return customers.Select(ToAdminDto).ToList();
        }

        public async Task<CustomerResponseDto?> GetCustomerById(int id)
        {
            var customer = await _repository.GetCustomerById(id);
            return customer == null ? null : ToDto(customer);
        }

        private static CustomerResponseDto ToDto(Customer c) => new CustomerResponseDto
        {
            CustomerId = c.CustomerId,
            FullName = c.FullName,
            Email = c.Email,
            Phone = c.Phone,
            Salary = c.Salary,
            CIBILScore = c.CIBILScore,
            EmploymentType = c.EmploymentType,
            CreatedDate = c.CreatedDate
        };

        private static AdminCustomerResponseDto ToAdminDto(Customer c) => new AdminCustomerResponseDto
        {
            CustomerId = c.CustomerId,
            FullName = c.FullName,
            Email = c.Email,
            Phone = c.Phone,
            Salary = c.Salary,
            CIBILScore = c.CIBILScore,
            EmploymentType = c.EmploymentType,
            CreatedDate = c.CreatedDate,
            AssignedAdminId = c.AssignedAdminId
        };

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

            if (dto.Phone.Distinct().Count() == 1)
               return (false, "Invalid phone number.", 0); 

            if (dto.Phone.Skip(1).Distinct().Count() == 1)
               return (false, "Invalid phone number.", 0);    

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
                CreatedDate = DateTime.Now,
                AssignedAdminId = await GetNextAdminInRotation(customers.Count)
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

        // Round-robin: admin1 gets customer #0, admin2 gets #1, admin3 gets #2,
        private async Task<int?> GetNextAdminInRotation(int existingCustomerCount)
        {
            var admins = await _userRepository.GetAllAdmins();
            if (admins.Count == 0)
                return null;

            var index = existingCustomerCount % admins.Count;
            return admins[index].UserId;
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

            var allCustomers = await _repository.GetAllCustomers();
            if (dto.Email != customer.Email && allCustomers.Any(c => c.Email == dto.Email))
                return (false, "Email already exists.");

            if (dto.Phone != customer.Phone && allCustomers.Any(c => c.Phone == dto.Phone))
                return (false, "Phone number already exists.");

            if (dto.Phone.Distinct().Count() == 1)
                 return (false, "Invalid phone number.");

            if (dto.Phone.Skip(1).Distinct().Count() == 1)
                  return (false, "Invalid phone number.");

                

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

        public async Task<bool> IsCustomerAssignedToAdmin(int customerId, int adminUserId)
        {
            var assignedId = await _repository.GetAssignedAdminId(customerId);
            return assignedId == adminUserId;
        }

        public async Task<List<int>> GetCustomerIdsForAdmin(int adminUserId) =>
            await _repository.GetCustomerIdsAssignedToAdmin(adminUserId);
    }
}