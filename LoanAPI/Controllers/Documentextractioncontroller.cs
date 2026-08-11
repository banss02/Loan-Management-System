using Microsoft.AspNetCore.Mvc;
using LoanAPI.Services;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentExtractionController : ControllerBase
    {
        private readonly DocumentExtractionService _documentExtractionService;

        public DocumentExtractionController(DocumentExtractionService documentExtractionService)
        {
            _documentExtractionService = documentExtractionService;
        }

        [HttpPost("extract")]
        public async Task<IActionResult> Extract(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    message = "Please upload a document."
                });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (extension != ".txt" && extension != ".pdf")
            {
                return BadRequest(new
                {
                    message = "Only .txt and .pdf files are supported."
                });
            }

            var extractedData = await _documentExtractionService.ExtractCustomerData(file);

            return Ok(extractedData);
        }
    }
}