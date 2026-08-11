using System.ComponentModel.DataAnnotations;
namespace LoanAPI.DTOs
{
    public class RegisterCustomerDto
    {
        public string FullName { get; set; } = "";
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$",
         ErrorMessage = "Email must be a valid Gmail address.")]
        public string Email { get; set; } = "";

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$",
         ErrorMessage = "Phone number must be 10 digits and start with 6, 7, 8, or 9.")]
        public string Phone { get; set; } = "";

        public decimal Salary { get; set; }
        public string EmploymentType { get; set; } = "";

        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public string? CompanyName { get; set; }
        public string? PANNumber { get; set; }
        public string? AadhaarNumber { get; set; }
        public string? GuardianName { get; set; }
        public string? Address { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
    }
}
