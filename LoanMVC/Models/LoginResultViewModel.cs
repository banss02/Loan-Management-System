namespace LoanMVC.Models
{
    // Must match the JSON shape returned by LoanAPI's POST api/User/login
    public class LoginResultViewModel
    {
        public string Token { get; set; } = "";
        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        public string Role { get; set; } = "";
        public string Username { get; set; } = "";
    }
}
