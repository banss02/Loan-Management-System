namespace LoanMVC.Models
{
    public class LoanScheduleViewModel
    {
        public int ScheduleId { get; set; }
        public int LoanId { get; set; }
        public int InstallmentNo { get; set; }
        public DateTime DueDate { get; set; }
        public decimal EMIAmount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public bool IsPaid { get; set; }
    }
}
