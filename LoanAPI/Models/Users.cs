namespace LoanAPI.Models
{
    // One login record per person. Role is either "Customer" or "Admin".
    // CustomerId is null for Admin accounts (an admin isn't a customer).
    //
    // Which customers an admin can see/manage is NOT stored here anymore - it's
    // stored on the Customer (AssignedAdminId), set at registration time in a
    // round-robin across whatever admins exist. See CustomerService.RegisterCustomer.
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Customer";
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string? SessionId { get; set; }
        public DateTime? SessionExpiresAt { get; set; }
    }
}