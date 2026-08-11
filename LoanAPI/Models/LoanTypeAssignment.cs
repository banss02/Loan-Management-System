using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanAPI.Models
{
    public class LoanTypeAssignment
    {
        [Key]
        public string LoanType { get; set; } = "";

        [ForeignKey(nameof(Admin))]
        public int UserId { get; set; }

        public User? Admin { get; set; }
    }
}