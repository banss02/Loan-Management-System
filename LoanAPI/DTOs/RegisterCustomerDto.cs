using System.ComponentModel.DataAnnotations;
namespace LoanAPI.DTOs
{
    public class RegisterCustomerDto
    {
        public string FullName { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$",
         ErrorMessage = "Email must be a valid Gmail address.")]
        public string Email { get; set; } = "";

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$",
         ErrorMessage = "Phone number must be 10 digits and start with 6, 7, 8, or 9.")]
        public string Phone { get; set; } = "";
        public decimal Salary { get; set; }
        public int CIBILScore { get; set; }
        public string EmploymentType { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
