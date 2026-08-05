namespace LoanMVC.Models
{
    // Matches LoanAPI's AdminCustomerResponseDto - only used on the admin customer list.
    // The plain CustomerViewModel (used for a customer's own profile) intentionally
    // does not have AssignedAdminId - that's internal routing info, not customer-facing.
    public class AdminCustomerViewModel
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public decimal Salary { get; set; }
        public int CIBILScore { get; set; }
        public string EmploymentType { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public int? AssignedAdminId { get; set; }
    }
}