using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanAPI.Services;
using LoanAPI.Helper;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;
        private readonly AccessControlService _access;

        public DocumentController(DocumentService documentService, AccessControlService access)
        {
            _documentService = documentService;
            _access = access;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var docs = await _documentService.GetAll();
            var visibleIds = await _access.GetVisibleCustomerIds(User);
            var visible = docs.Where(d => visibleIds.Contains(d.CustomerId)).ToList();
            return Ok(visible);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            if (!await _access.CanAccessCustomer(User, customerId))
                return Forbid();

            var docs = await _documentService.GetByCustomerId(customerId);
            return Ok(docs);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var myCustomerId = AccessControlService.GetMyCustomerId(User);
            if (myCustomerId == null && !AccessControlService.IsAdmin(User))
                return Forbid();

            var customerId = myCustomerId ?? 0;
            var result = await _documentService.Upload(customerId, file);
            return Ok(result);
        }
    }
}