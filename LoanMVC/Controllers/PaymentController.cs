using Microsoft.AspNetCore.Mvc;
using LoanMVC.Services;
using LoanMVC.Models;
using LoanMVC.Filters;

namespace LoanMVC.Controllers
{
    [SessionAuth]
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                var allPayments = await _paymentService.GetPayments();
                return View(allPayments);
            }

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var myPayments = await _paymentService.GetPaymentsByCustomerId(customerId.Value);
            return View(myPayments);
        }

        public IActionResult Create(int loanId, int? scheduleId = null, decimal? amount = null)
        {
            return View(new PaymentViewModel
            {
                LoanId = loanId,
                ScheduleId = scheduleId,
                Amount = amount ?? 0
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _paymentService.MakePayment(model);
            if (!success)
            {
                ModelState.AddModelError("", "Payment failed.");
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}