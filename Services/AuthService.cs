using ExpenseTracker.DTOs.AuthDtos;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Cryptography;

namespace ExpenseTracker.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jWTService;

        public AuthService(IUserRepository userRepository, IJWTService jWTService)
        {
            _userRepository = userRepository;
            _jWTService = jWTService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto login)
        {
            var user = await _userRepository.GetByUserNameAsync(login.Username);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            
            if (!VerifyPasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new UnauthorizedAccessException("Invalid password");
            }

            var token = _jWTService.GenerateToken(user);

            return new LoginResponseDto 
            {
                UserId = user.Id,
                Username = user.Username,
                Nickname = user.NickName,
                Email = user.Email,
                Token = token
            };

        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
}
