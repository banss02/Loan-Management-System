using Microsoft.AspNetCore.Mvc;
using LoanMVC.Services;
using LoanMVC.Models;
using LoanMVC.Filters;

namespace LoanMVC.Controllers
{
    [SessionAuth] 
    public class LoanController : Controller
    {
        private readonly LoanService _loanService;
        private readonly LoanScheduleService _scheduleService;

        public LoanController(LoanService loanService, LoanScheduleService scheduleService)
        {
            _loanService = loanService;
            _scheduleService = scheduleService;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                var allLoans = await _loanService.GetLoans();
                return View(allLoans);
            }

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var myLoans = await _loanService.GetLoansByCustomerId(customerId.Value);
            return View(myLoans);
        }

        public IActionResult Apply()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Apply(ApplyLoanViewModel model)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            model.CustomerId = customerId.Value;
            ModelState.Remove(nameof(model.CustomerId));

            if (!ModelState.IsValid)
                return View(model);

            var result = await _loanService.ApplyLoan(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var loan = await _loanService.GetLoanById(id);
            if (loan == null)
                return NotFound();

            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                var customerId = HttpContext.Session.GetInt32("CustomerId");
                if (loan.CustomerId != customerId)
                    return Forbid();
            }

            if (loan.Status == "Approved")
            {
                var schedule = await _scheduleService.GetScheduleByLoanId(id);
                var totalEmis = schedule.Count;
                var paidEmis = schedule.Count(s => s.IsPaid);

                ViewBag.TotalEmis = totalEmis;
                ViewBag.PaidEmis = paidEmis;
                ViewBag.ProgressPercent = totalEmis > 0 ? (int)Math.Round(paidEmis * 100.0 / totalEmis) : 0;
            }

            return View(loan);
        }

        [HttpPost]
        [SessionAuth(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _loanService.ApproveLoan(id);
            if (!success)
                TempData["Error"] = "Unable to approve loan.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [SessionAuth(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var success = await _loanService.RejectLoan(id);
            if (!success)
                TempData["Error"] = "Unable to reject loan.";

            return RedirectToAction("Index");
        }
    }
}