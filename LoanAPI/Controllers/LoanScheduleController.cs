using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanAPI.Services;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoanScheduleController : ControllerBase
    {
        private readonly LoanScheduleService _scheduleService;
        private readonly LoanService _loanService;

        public LoanScheduleController(LoanScheduleService scheduleService, LoanService loanService)
        {
            _scheduleService = scheduleService;
            _loanService = loanService;
        }

        [HttpGet("loan/{loanId}")]
        public async Task<IActionResult> GetByLoanId(int loanId)
        {
            var loan = await _loanService.GetLoanEntityById(loanId);
            if (loan == null)
                return NotFound();

            var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";
            var myCustomerId = int.TryParse(User.FindFirst("CustomerId")?.Value, out var id) ? id : (int?)null;

            if (!isAdmin && loan.CustomerId != myCustomerId)
                return Forbid();

            var schedule = await _scheduleService.GetByLoanId(loanId);
            return Ok(schedule);
        }
    }
}
