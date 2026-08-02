using System.ComponentModel.DataAnnotations;

namespace LoanMVC.Models
{
    // Matches LoanAPI's RegisterCustomerDto - sent to POST api/Customer
    public class RegisterCustomerViewModel
    {
        [Required]
        public string FullName { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Phone { get; set; } = "";

        [Required]
        public decimal Salary { get; set; }

        [Required]
        public int CIBILScore { get; set; }

        [Required]
        public string EmploymentType { get; set; } = "";

        [Required]
        public string Username { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
