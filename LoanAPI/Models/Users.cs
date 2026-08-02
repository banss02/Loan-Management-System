using System.ComponentModel.DataAnnotations;

namespace LoanAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Customer";
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
