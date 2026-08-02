using LoanAPI.Models;
using LoanAPI.Repositories;
using LoanAPI.DTOs;

namespace LoanAPI.Services
{
    public class DocumentService
    {
        private readonly DocumentRepository _repository;
        private readonly IWebHostEnvironment _env;

        public DocumentService(DocumentRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        public async Task<List<DocumentResponseDto>> GetByCustomerId(int customerId)
        {
            var docs = await _repository.GetByCustomerId(customerId);
            return docs.Select(ToDto).ToList();
        }

        // Admin only - every document across all customers
        public async Task<List<DocumentResponseDto>> GetAll()
        {
            var docs = await _repository.GetAll();
            return docs.Select(ToDto).ToList();
        }

        private static DocumentResponseDto ToDto(Document d) => new DocumentResponseDto
        {
            DocumentId = d.DocumentId,
            CustomerId = d.CustomerId,
            DocumentName = d.DocumentName,
            UploadedDate = d.UploadedDate
        };

        public async Task<DocumentResponseDto> Upload(int customerId, IFormFile file)
        {
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "Uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{customerId}_{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Document
            {
                CustomerId = customerId,
                DocumentName = file.FileName,
                FilePath = filePath,
                UploadedDate = DateTime.Now
            };

            await _repository.AddDocument(document);

            return new DocumentResponseDto
            {
                DocumentId = document.DocumentId,
                CustomerId = document.CustomerId,
                DocumentName = document.DocumentName,
                UploadedDate = document.UploadedDate
            };
        }
    }
}