namespace LoanAPI.DTOs
{
    public class DocumentResponseDto
    {
        public int DocumentId { get; set; }
        public int CustomerId { get; set; }
        public string DocumentName { get; set; } = "";
        public DateTime UploadedDate { get; set; }
    }
}
