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
    public class LoanController : ControllerBase
    {
        private readonly LoanService _loanService;

        public LoanController(LoanService loanService)
        {
            _loanService = loanService;
        }

        private int? MyCustomerId =>
            int.TryParse(User.FindFirst("CustomerId")?.Value, out var id) ? id : null;

        private bool IsAdmin => User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var loans = await _loanService.GetAllLoans();
            return Ok(loans);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            if (!IsAdmin && MyCustomerId != customerId)
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

            if (!IsAdmin && loan.CustomerId != MyCustomerId)
                return Forbid();

            var dto = await _loanService.GetLoanById(id);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Apply(ApplyLoanDto dto)
        {
            
            if (!IsAdmin)
            {
                if (MyCustomerId == null)
                    return Forbid();

                dto.CustomerId = MyCustomerId.Value;
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
            var success = await _loanService.ApproveLoan(id);
            if (!success)
                return BadRequest(new { message = "Unable to approve loan (not found or not pending)." });

            return Ok(new { message = "Loan approved." });
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var success = await _loanService.RejectLoan(id);
            if (!success)
                return BadRequest(new { message = "Unable to reject loan (not found or not pending)." });

            return Ok(new { message = "Loan rejected." });
        }
    }
}
