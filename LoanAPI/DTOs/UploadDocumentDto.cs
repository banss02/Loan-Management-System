namespace LoanAPI.DTOs
{
    public class UploadDocumentDto
    {
        public int CustomerId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
