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
        public async Task<IActionResult> Register(RegisterCustomerViewModel model, IFormFile? documentFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _customerService.Register(model);

            if(result.Success && documentFile != null)
            {
                await _customerService.UploadDocument(
                    result.CustomerId,
                     documentFile);
            }

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


    var missingFields = new List<string>();

    if (string.IsNullOrWhiteSpace(customer.CompanyName))
        missingFields.Add("Company Name");

    if (string.IsNullOrWhiteSpace(customer.PANNumber))
        missingFields.Add("PAN Number");

    if (string.IsNullOrWhiteSpace(customer.AadhaarNumber))
        missingFields.Add("Aadhaar Number");

    if (string.IsNullOrWhiteSpace(customer.GuardianName))
        missingFields.Add("Guardian Name");

    if (string.IsNullOrWhiteSpace(customer.Address))
        missingFields.Add("Address");

    if (string.IsNullOrWhiteSpace(customer.BankName))
        missingFields.Add("Bank Name");

    if (string.IsNullOrWhiteSpace(customer.AccountNumber))
        missingFields.Add("Account Number");

    if (string.IsNullOrWhiteSpace(customer.IFSCCode))
        missingFields.Add("IFSC Code");


    ViewBag.MissingFields = missingFields;


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
        EmploymentType = customer.EmploymentType,
        CompanyName = customer.CompanyName,
        Address = customer.Address,
        GuardianName = customer.GuardianName,
        PANNumber = customer.PANNumber,
        AadhaarNumber = customer.AadhaarNumber,
        BankName = customer.BankName,
        AccountNumber = customer.AccountNumber,
        IFSCCode = customer.IFSCCode
    };

    var missingFields = new List<string>();

    if (string.IsNullOrWhiteSpace(model.CompanyName))
        missingFields.Add("Company Name");

    if (string.IsNullOrWhiteSpace(model.PANNumber))
        missingFields.Add("PAN Number");

    if (string.IsNullOrWhiteSpace(model.AadhaarNumber))
        missingFields.Add("Aadhaar Number");

    if (string.IsNullOrWhiteSpace(model.GuardianName))
        missingFields.Add("Guardian Name");

    if (string.IsNullOrWhiteSpace(model.Address))
        missingFields.Add("Address");

    if (string.IsNullOrWhiteSpace(model.BankName))
        missingFields.Add("Bank Name");

    if (string.IsNullOrWhiteSpace(model.AccountNumber))
        missingFields.Add("Account Number");

    if (string.IsNullOrWhiteSpace(model.IFSCCode))
        missingFields.Add("IFSC Code");

    ViewBag.MissingFields = missingFields;

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