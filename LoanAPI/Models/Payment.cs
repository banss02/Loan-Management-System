using System.ComponentModel.DataAnnotations;

namespace LoanAPI.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int LoanId { get; set; }
        public Loan? Loan { get; set; }
        public int? ScheduleId { get; set; }
        public LoanSchedule? Schedule { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
