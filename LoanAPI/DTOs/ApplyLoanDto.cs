namespace LoanAPI.DTOs
{
    public class ApplyLoanDto
    {
        public int CustomerId { get; set; }
        public string LoanType { get; set; } = "";
        public decimal LoanAmount { get; set; }
        public int TenureMonths { get; set; }
    }
}
