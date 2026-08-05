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

        public IActionResult Login(string? reason)
        {
            if (reason == "expired")
            {
                ViewBag.SessionExpiredMessage =
                    "You have been logged out because this account was signed in from another browser or device.";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (result, errorMessage) = await _accountService.Login(model);

            if (result == null)
            {
                ModelState.AddModelError("", errorMessage ?? "Invalid username or password.");
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

        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}