namespace LoanMVC.Models
{
    public class ExtractedCustomerDataViewModel
    {
        public string? FullName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public decimal? Salary { get; set; }

        public string? EmploymentType { get; set; }

        public string? CompanyName { get; set; }

        public string? PANNumber { get; set; }

        public string? AadhaarNumber { get; set; }

        public string? GuardianName { get; set; }

        public string? Address { get; set; }

        public string? BankName { get; set; }

        public string? AccountNumber { get; set; }

        public string? IFSCCode { get; set; }

        public int FieldsFound { get; set; }
    }
}