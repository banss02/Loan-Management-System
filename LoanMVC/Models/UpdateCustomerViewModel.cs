using System.ComponentModel.DataAnnotations;

namespace LoanMVC.Models
{
    public class UpdateCustomerViewModel
    {
        public int CustomerId { get; set; }
        [Required]
        public string FullName { get; set; } = "";
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        public string Phone { get; set; } = "";
        [Required]
        public decimal Salary { get; set; }
        [Required]
        public string EmploymentType { get; set; } = "";
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