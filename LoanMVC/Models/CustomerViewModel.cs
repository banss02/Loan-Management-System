namespace LoanMVC.Models
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public decimal Salary { get; set; }
        public int CIBILScore { get; set; }
        public string EmploymentType { get; set; } = "";
        public DateTime CreatedDate { get; set; }
    }
}