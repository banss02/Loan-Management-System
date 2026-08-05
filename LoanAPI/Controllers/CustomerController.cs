using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanAPI.DTOs;
using LoanAPI.Services;
using LoanAPI.Helper;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;
        private readonly LoanService _loanService;
        private readonly AccessControlService _access;

        public CustomerController(CustomerService customerService, LoanService loanService, AccessControlService access)
        {
            _customerService = customerService;
            _loanService = loanService;
            _access = access;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterCustomerDto dto)
        {
            var result = await _customerService.RegisterCustomer(dto);

            if (!result.Success)
                return BadRequest(new RegisterCustomerResponseDto { CustomerId = 0, Message = result.Message });

            return Ok(new RegisterCustomerResponseDto { CustomerId = result.CustomerId, Message = result.Message });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllCustomersForAdmin();
            var visibleIds = await _access.GetVisibleCustomerIds(User);
            var visible = customers.Where(c => visibleIds.Contains(c.CustomerId)).ToList();
            return Ok(visible);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            if (!await _access.CanAccessCustomer(User, id))
                return Forbid();

            var customer = await _customerService.GetCustomerById(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UpdateCustomerDto dto)
        {
            if (!await _access.CanAccessCustomer(User, id))
                return Forbid();

            var result = await _customerService.UpdateCustomer(id, dto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _access.CanAccessCustomer(User, id))
                return Forbid();

            var result = await _customerService.DeleteCustomer(id);
            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("{id}/loans")]
        [Authorize]
        public async Task<IActionResult> GetLoans(int id)
        {
            if (!await _access.CanAccessCustomer(User, id))
                return Forbid();

            var loans = await _loanService.GetLoansByCustomerId(id);
            return Ok(loans);
        }
    }
}