using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanAPI.Services;
using LoanAPI.Helper;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoanScheduleController : ControllerBase
    {
        private readonly LoanScheduleService _scheduleService;
        private readonly LoanService _loanService;
        private readonly AccessControlService _access;

        public LoanScheduleController(LoanScheduleService scheduleService, LoanService loanService, AccessControlService access)
        {
            _scheduleService = scheduleService;
            _loanService = loanService;
            _access = access;
        }

        [HttpGet("loan/{loanId}")]
        public async Task<IActionResult> GetByLoanId(int loanId)
        {
            var loan = await _loanService.GetLoanEntityById(loanId);
            if (loan == null)
                return NotFound();

            if (!await _access.CanAccessCustomer(User, loan.CustomerId))
                return Forbid();

            var schedule = await _scheduleService.GetByLoanId(loanId);
            return Ok(schedule);
        }
    }
}