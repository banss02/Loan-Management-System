using Microsoft.AspNetCore.Mvc;
using LoanMVC.Services;
using LoanMVC.Filters;

namespace LoanMVC.Controllers
{
    [SessionAuth]
    public class DocumentController : Controller
    {
        private readonly DocumentService _documentService;

        public DocumentController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                var allDocs = await _documentService.GetAll();
                return View(allDocs);
            }

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var myDocs = await _documentService.GetByCustomerId(customerId.Value);
            return View(myDocs);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please choose a file.");
                return View();
            }

            var success = await _documentService.Upload(file);
            if (!success)
            {
                ModelState.AddModelError("", "Upload failed.");
                return View();
            }

            TempData["Success"] = "Document uploaded successfully.";
            return RedirectToAction("Index");
        }
    }
}