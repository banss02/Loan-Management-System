using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanAPI.DTOs;
using LoanAPI.Services;
using LoanAPI.Helper;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly LoanService _loanService;
        private readonly AccessControlService _access;

        public PaymentController(PaymentService paymentService, LoanService loanService, AccessControlService access)
        {
            _paymentService = paymentService;
            _loanService = loanService;
            _access = access;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentService.GetAllPayments();
            var visibleIds = await _access.GetVisibleCustomerIds(User);
            var visible = payments.Where(p => visibleIds.Contains(p.CustomerId)).ToList();
            return Ok(visible);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            if (!await _access.CanAccessCustomer(User, customerId))
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

            if (!await _access.CanAccessCustomer(User, loan.CustomerId))
                return Forbid();

            var result = await _paymentService.MakePayment(dto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}