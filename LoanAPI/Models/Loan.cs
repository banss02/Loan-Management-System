using System.ComponentModel.DataAnnotations;

namespace LoanAPI.Models
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string LoanType { get; set; } = "";
        public decimal LoanAmount { get; set; }
        public int TenureMonths { get; set; }
        public decimal InterestRate { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime AppliedDate { get; set; }
    }
}
