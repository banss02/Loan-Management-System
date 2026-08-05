using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanAPI.DTOs;
using LoanAPI.Services;
using LoanAPI.Helper;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]     public class LoanController : ControllerBase
    {
        private readonly LoanService _loanService;
        private readonly AccessControlService _access;

        public LoanController(LoanService loanService, AccessControlService access)
        {
            _loanService = loanService;
            _access = access;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var loans = await _loanService.GetAllLoans();
            var visibleIds = await _access.GetVisibleCustomerIds(User);
            var visible = loans.Where(l => visibleIds.Contains(l.CustomerId)).ToList();
            return Ok(visible);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            if (!await _access.CanAccessCustomer(User, customerId))
                return Forbid();

            var loans = await _loanService.GetLoansByCustomerId(customerId);
            return Ok(loans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var loan = await _loanService.GetLoanEntityById(id);
            if (loan == null)
                return NotFound();

            if (!await _access.CanAccessCustomer(User, loan.CustomerId))
                return Forbid();

            var dto = await _loanService.GetLoanById(id);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Apply(ApplyLoanDto dto)
        {
            if (!AccessControlService.IsAdmin(User))
            {
                var myCustomerId = AccessControlService.GetMyCustomerId(User);
                if (myCustomerId == null)
                    return Forbid();

                dto.CustomerId = myCustomerId.Value;
            }

            var result = await _loanService.ApplyLoan(dto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var loan = await _loanService.GetLoanEntityById(id);
            if (loan == null)
                return NotFound();

            if (!await _access.CanAccessCustomer(User, loan.CustomerId))
                return Forbid();

            var success = await _loanService.ApproveLoan(id);
            if (!success)
                return BadRequest(new { message = "Unable to approve loan (not found or not pending)." });

            return Ok(new { message = "Loan approved." });
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var loan = await _loanService.GetLoanEntityById(id);
            if (loan == null)
                return NotFound();

            if (!await _access.CanAccessCustomer(User, loan.CustomerId))
                return Forbid();

            var success = await _loanService.RejectLoan(id);
            if (!success)
                return BadRequest(new { message = "Unable to reject loan (not found or not pending)." });

            return Ok(new { message = "Loan rejected." });
        }
    }
}