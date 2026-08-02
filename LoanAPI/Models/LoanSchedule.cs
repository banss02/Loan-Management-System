using System.ComponentModel.DataAnnotations;

namespace LoanAPI.Models
{
    public class LoanSchedule
    {
        [Key]
        public int ScheduleId { get; set; }
        public int LoanId { get; set; }
        public Loan? Loan { get; set; }
        public int InstallmentNo { get; set; }
        public DateTime DueDate { get; set; }
        public decimal EMIAmount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public bool IsPaid { get; set; }
    }
}
