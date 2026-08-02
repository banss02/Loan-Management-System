using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanAPI.DTOs;
using LoanAPI.Services;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly LoanService _loanService;

        public PaymentController(PaymentService paymentService, LoanService loanService)
        {
            _paymentService = paymentService;
            _loanService = loanService;
        }

        private bool IsAdmin => User.FindFirst(ClaimTypes.Role)?.Value == "Admin";
        private int? MyCustomerId =>
            int.TryParse(User.FindFirst("CustomerId")?.Value, out var id) ? id : null;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentService.GetAllPayments();
            return Ok(payments);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            if (!IsAdmin && MyCustomerId != customerId)
                return Forbid();

            var payments = await _paymentService.GetPaymentsByCustomerId(customerId);
            return Ok(payments);
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment(PaymentDto dto)
        {
            var loan = await _loanService.GetLoanEntityById(dto.LoanId);
            if (loan == null)
                return NotFound();

            if (!IsAdmin && loan.CustomerId != MyCustomerId)
                return Forbid();

            var result = await _paymentService.MakePayment(dto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}
