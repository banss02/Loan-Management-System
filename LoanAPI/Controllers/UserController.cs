using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanAPI.DTOs;
using LoanAPI.Services;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _userService.Login(dto);

            if (!result.Success)
            {
                if (result.ErrorMessage.Contains("already logged in"))
                    return Conflict(new { message = result.ErrorMessage });

                return Unauthorized(new { message = result.ErrorMessage });
            }

            return Ok(result.Data);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                await _userService.Logout(userId);
            }

            return Ok(new { message = "Logged out." });
        }
    }
}