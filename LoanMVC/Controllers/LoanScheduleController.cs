using Microsoft.AspNetCore.Mvc;
using LoanMVC.Services;
using LoanMVC.Filters;

namespace LoanMVC.Controllers
{
    [SessionAuth]
    public class LoanScheduleController : Controller
    {
        private readonly LoanScheduleService _scheduleService;
        private readonly LoanService _loanService;

        public LoanScheduleController(LoanScheduleService scheduleService, LoanService loanService)
        {
            _scheduleService = scheduleService;
            _loanService = loanService;
        }

        public async Task<IActionResult> Index(int loanId)
        {
            var loan = await _loanService.GetLoanById(loanId);
            if (loan == null)
                return NotFound();

            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                var customerId = HttpContext.Session.GetInt32("CustomerId");
                if (loan.CustomerId != customerId)
                    return Forbid();
            }

            var schedule = await _scheduleService.GetScheduleByLoanId(loanId);
            ViewBag.LoanId = loanId;
            return View(schedule);
        }
    }
}
