namespace LoanAPI.DTOs
{
    public class LoanResponseDto
    {
        public int LoanId { get; set; }
        public int CustomerId { get; set; }
        public string LoanType { get; set; } = "";
        public decimal LoanAmount { get; set; }
        public int TenureMonths { get; set; }
        public decimal InterestRate { get; set; }
        public string Status { get; set; } = "";
        public DateTime AppliedDate { get; set; }
    }
}
