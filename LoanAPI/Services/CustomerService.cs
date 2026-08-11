using LoanAPI.Models;
using LoanAPI.Repositories;
using LoanAPI.DTOs;
using LoanAPI.Helper;

namespace LoanAPI.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _repository;
        private readonly UserRepository _userRepository;

        private readonly EncryptionService _encryptionService;

        public CustomerService(CustomerRepository repository, UserRepository userRepository, EncryptionService encryptionService)
        {
            _repository = repository;
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }

        public async Task<List<AdminCustomerResponseDto>> GetAllCustomersForAdmin()
        {
            var customers = await _repository.GetAllCustomers();
            var result = new List<AdminCustomerResponseDto>();

            foreach (var customer in customers)
            {
                  var userId = await _userRepository.GetUserIdByCustomerId(customer.CustomerId);
                  result.Add(ToAdminDto(customer, userId));
            }

            return result;
        }

        public async Task<CustomerResponseDto?> GetCustomerById(int id)
        {
            var customer = await _repository.GetCustomerById(id);

            if (customer == null)
                   return null;

            var userId = await _userRepository.GetUserIdByCustomerId(id);

            return ToDto(customer, userId);
        }

        private CustomerResponseDto ToDto(Customer c, int? userId) => new CustomerResponseDto
        {
            CustomerId = c.CustomerId,
            FullName = c.FullName,
            DateOfBirth = c.DateOfBirth,
            Email = c.Email,
            Phone = c.Phone,
            Salary = c.Salary,
            EmploymentType = c.EmploymentType,
            CreatedDate = c.CreatedDate,
            CompanyName = c.CompanyName,
            PANNumber = userId.HasValue? _encryptionService.Decrypt(c.PANNumber, userId.Value): c.PANNumber,
            AadhaarNumber = userId.HasValue? _encryptionService.Decrypt(c.AadhaarNumber, userId.Value): c.AadhaarNumber,
            Address = c.Address,
            GuardianName = c.GuardianName,
            BankName = c.BankName,
            AccountNumber = userId.HasValue? _encryptionService.Decrypt(c.AccountNumber, userId.Value): c.AccountNumber,
            IFSCCode = c.IFSCCode,

        };

        private AdminCustomerResponseDto ToAdminDto(Customer c, int? userId) => new AdminCustomerResponseDto
        {
            CustomerId = c.CustomerId,
            FullName = c.FullName,
            DateOfBirth = c.DateOfBirth,
            Email = c.Email,
            Phone = c.Phone,
            Salary = c.Salary,
            EmploymentType = c.EmploymentType,
            CompanyName = c.CompanyName,
            PANNumber = userId.HasValue? _encryptionService.Decrypt(c.PANNumber, userId.Value): c.PANNumber,
            AadhaarNumber = userId.HasValue? _encryptionService.Decrypt(c.AadhaarNumber, userId.Value): c.AadhaarNumber,
            GuardianName = c.GuardianName,
            Address = c.Address,
            BankName = c.BankName,
            AccountNumber = userId.HasValue? _encryptionService.Decrypt(c.AccountNumber, userId.Value): c.AccountNumber,
            IFSCCode = c.IFSCCode,
            CreatedDate = c.CreatedDate
        };

        public async Task<(bool Success, string Message, int CustomerId)> RegisterCustomer(RegisterCustomerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return (false, "Full Name is required.", 0);

            int age = DateTime.Now.Year - dto.DateOfBirth.Year;
            if (dto.DateOfBirth > DateOnly.FromDateTime(DateTime.Now.AddYears(-age)))
                age--;

            if (age < 21)
                return (false, "Customer must be at least 21 years old.", 0);

            if (dto.Salary <= 0)
                return (false, "Salary must be greater than zero.", 0);

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
                EmploymentType = dto.EmploymentType,
                CreatedDate = DateTime.Now,
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
            

            customer.CompanyName = dto.CompanyName;
            customer.PANNumber = _encryptionService.Encrypt(dto.PANNumber, user.UserId);
            customer.AadhaarNumber = _encryptionService.Encrypt(dto.AadhaarNumber, user.UserId);
            customer.GuardianName = dto.GuardianName;
            customer.Address = dto.Address;
            customer.BankName = dto.BankName;
            customer.AccountNumber = _encryptionService.Encrypt(dto.AccountNumber, user.UserId);
            customer.IFSCCode = dto.IFSCCode;

            await _repository.UpdateCustomer(customer);

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

            var employment = dto.EmploymentType.ToLower();
            if (employment != "salaried" && employment != "self-employed" && employment != "business")
                return (false, "Invalid Employment Type.");

            var allCustomers = await _repository.GetAllCustomers();
            if (dto.Email != customer.Email && allCustomers.Any(c => c.Email == dto.Email))
                return (false, "Email already exists.");
            if (dto.Phone != customer.Phone && allCustomers.Any(c => c.Phone == dto.Phone))
                return (false, "Phone number already exists.");

            var userId = await _userRepository.GetUserIdByCustomerId(id);

            if (userId == null)
                return (false, "User not found.");

            customer.FullName = dto.FullName;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.Salary = dto.Salary;
            customer.EmploymentType = dto.EmploymentType;
            customer.CompanyName = dto.CompanyName;
            customer.PANNumber = _encryptionService.Encrypt(dto.PANNumber, userId.Value);
            customer.AadhaarNumber = _encryptionService.Encrypt(dto.AadhaarNumber, userId.Value);
            customer.GuardianName = dto.GuardianName;
            customer.Address = dto.Address;
            customer.BankName = dto.BankName;
            customer.AccountNumber = _encryptionService.Encrypt(dto.AccountNumber, userId.Value);
            customer.IFSCCode = dto.IFSCCode;   


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