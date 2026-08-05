namespace LoanAPI.DTOs
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int LoanId { get; set; }
        public int CustomerId { get; set; }
        public int? ScheduleId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
