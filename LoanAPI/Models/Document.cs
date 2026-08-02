using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LoanAPI.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string DocumentName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public DateTime UploadedDate { get; set; }
    }
}
