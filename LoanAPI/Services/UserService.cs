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

        public async Task<LoginResponseDto?> Login(LoginDto dto)
        {
            var user = await _userRepository.GetUserByUsername(dto.Username);

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            var token = _tokenService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                UserId = user.UserId,
                CustomerId = user.CustomerId,
                Role = user.Role,
                Username = user.Username
            };
        }
    }
}
