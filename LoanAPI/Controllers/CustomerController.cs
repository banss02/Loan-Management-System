using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanAPI.DTOs;
using LoanAPI.Services;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;
        private readonly LoanService _loanService;

        public CustomerController(CustomerService customerService, LoanService loanService)
        {
            _customerService = customerService;
            _loanService = loanService;
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
            var customers = await _customerService.GetAllCustomers();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            if (!IsSelfOrAdmin(id))
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
            if (!IsSelfOrAdmin(id))
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
            var result = await _customerService.DeleteCustomer(id);
            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("{id}/loans")]
        [Authorize]
        public async Task<IActionResult> GetLoans(int id)
        {
            if (!IsSelfOrAdmin(id))
                return Forbid();

            var loans = await _loanService.GetLoansByCustomerId(id);
            return Ok(loans);
        }

        private bool IsSelfOrAdmin(int customerId)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var myCustomerId = User.FindFirst("CustomerId")?.Value;

            return role == "Admin" || myCustomerId == customerId.ToString();
        }
    }
}