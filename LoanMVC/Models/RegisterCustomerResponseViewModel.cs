namespace LoanMVC.Models
{
    // Matches LoanAPI's RegisterCustomerResponseDto
    public class RegisterCustomerResponseViewModel
    {
        public int CustomerId { get; set; }
        public string Message { get; set; } = "";
    }
}
