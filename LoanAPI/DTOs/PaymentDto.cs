namespace LoanAPI.DTOs
{
    public class PaymentDto
    {
        public int LoanId { get; set; }
        public int? ScheduleId { get; set; }
        public decimal Amount { get; set; }
    }
}
