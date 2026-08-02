using Microsoft.AspNetCore.Mvc;
using LoanMVC.Services;
using LoanMVC.Models;

namespace LoanMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.Login(model);

            if (result == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetInt32("UserId", result.UserId);
            if (result.CustomerId.HasValue)
                HttpContext.Session.SetInt32("CustomerId", result.CustomerId.Value);
            HttpContext.Session.SetString("Role", result.Role);
            HttpContext.Session.SetString("Username", result.Username);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
