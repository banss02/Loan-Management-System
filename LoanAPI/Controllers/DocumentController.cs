using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanAPI.Services;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;

        public DocumentController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        private bool IsAdmin => User.FindFirst(ClaimTypes.Role)?.Value == "Admin";
        private int? MyCustomerId =>
            int.TryParse(User.FindFirst("CustomerId")?.Value, out var id) ? id : null;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var docs = await _documentService.GetAll();
            return Ok(docs);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            if (!IsAdmin && MyCustomerId != customerId)
                return Forbid();

            var docs = await _documentService.GetByCustomerId(customerId);
            return Ok(docs);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (MyCustomerId == null && !IsAdmin)
                return Forbid();

            var customerId = MyCustomerId ?? 0;
            var result = await _documentService.Upload(customerId, file);
            return Ok(result);
        }
    }
}