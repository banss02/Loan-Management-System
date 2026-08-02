using System.ComponentModel.DataAnnotations;

namespace LoanMVC.Models
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }

        [Required]
        public int LoanId { get; set; }

        public int? ScheduleId { get; set; }

        [Required]
        [Range(1, 100000000)]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
