using System.ComponentModel.DataAnnotations;

namespace LoanMVC.Models
{
    public class UpdateCustomerViewModel
    {
        public int CustomerId { get; set; }

        [Required]
        public string FullName { get; set; } = "";

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
    }
}