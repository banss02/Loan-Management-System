using System.ComponentModel.DataAnnotations;

namespace LoanMVC.Models
{
    public class ApplyLoanViewModel
    {
        public int CustomerId { get; set; }

        [Required]
        public string LoanType { get; set; } = "";

        [Required]
        [Range(1000, 100000000)]
        public decimal LoanAmount { get; set; }

        [Required]
        [Range(1, 360)]
        public int TenureMonths { get; set; }
    }
}
