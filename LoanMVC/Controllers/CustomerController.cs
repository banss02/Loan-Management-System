using Microsoft.AspNetCore.Mvc;
using LoanMVC.Services;
using LoanMVC.Models;
using LoanMVC.Filters;

namespace LoanMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterCustomerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _customerService.Register(model);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Login", "Account");
        }

        [SessionAuth]
        public async Task<IActionResult> Details()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var customer = await _customerService.GetCustomerById(customerId.Value);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [SessionAuth(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomers();
            return View(customers);
        }

        [SessionAuth]
        public async Task<IActionResult> Edit()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var customer = await _customerService.GetCustomerById(customerId.Value);
            if (customer == null)
                return NotFound();

            var model = new UpdateCustomerViewModel
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                Salary = customer.Salary,
                CIBILScore = customer.CIBILScore,
                EmploymentType = customer.EmploymentType
            };

            return View(model);
        }

        [HttpPost]
        [SessionAuth]
        public async Task<IActionResult> Edit(UpdateCustomerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account");

            var result = await _customerService.UpdateCustomer(customerId.Value, model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Details");
        }

        [HttpPost]
        [SessionAuth(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _customerService.DeleteCustomer(id);
            if (!success)
                TempData["Error"] = "Unable to delete customer.";

            return RedirectToAction("Index");
        }
    }
}