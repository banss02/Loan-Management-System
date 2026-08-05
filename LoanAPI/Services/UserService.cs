using LoanAPI.DTOs;
using LoanAPI.Repositories;

namespace LoanAPI.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly TokenService _tokenService;

        public UserService(UserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<(bool Success, string ErrorMessage, LoginResponseDto? Data)> Login(LoginDto dto)
        {
            var user = await _userRepository.GetUserByUsername(dto.Username);

            if (user == null)
                return (false, "Invalid username or password.", null);

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return (false, "Invalid username or password.", null);

            var hasActiveSession = !string.IsNullOrEmpty(user.SessionId)
                                    && user.SessionExpiresAt.HasValue
                                    && user.SessionExpiresAt.Value > DateTime.UtcNow;

            if (hasActiveSession)
            {
                return (false, "This account is already logged in on another browser or device. Please log out from that session first.", null);
            }

            var sessionId = Guid.NewGuid().ToString();
            user.SessionId = sessionId;
            user.SessionExpiresAt = DateTime.UtcNow.AddMinutes(15); 
            await _userRepository.UpdateUser(user);

            var token = _tokenService.GenerateToken(user, sessionId);

            var data = new LoginResponseDto
            {
                Token = token,
                UserId = user.UserId,
                CustomerId = user.CustomerId,
                Role = user.Role,
                Username = user.Username
            };

            return (true, "", data);
        }

        public async Task Logout(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
                return;

            user.SessionId = null;
            user.SessionExpiresAt = null;
            await _userRepository.UpdateUser(user);
        }
    }
}