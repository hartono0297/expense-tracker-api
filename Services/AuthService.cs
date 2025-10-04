using ExpenseTracker.DTOs.AuthDtos;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Interfaces;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using ExpenseTracker.Data;

namespace ExpenseTracker.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jWTService;
        private readonly AppDbContext _dbContext;

        public AuthService(IUserRepository userRepository, IJWTService jWTService, AppDbContext dbContext)
        {
            _userRepository = userRepository;
            _jWTService = jWTService;
            _dbContext = dbContext;
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
            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };
            _dbContext.RefreshTokens.Add(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            return new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Nickname = user.NickName,
                Email = user.Email,
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<RefreshResponseDto> RefreshTokenAsync(RefreshRequestDto request)
        {
            var refreshTokenEntity = await _dbContext.RefreshTokens.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == request.RefreshToken && !r.IsRevoked && !r.IsUsed);
            if (refreshTokenEntity == null || refreshTokenEntity.Expires < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }
            // Mark old token as used
            refreshTokenEntity.IsUsed = true;
            refreshTokenEntity.IsRevoked = true;
            _dbContext.RefreshTokens.Update(refreshTokenEntity);

            // Issue new tokens
            var newJwt = _jWTService.GenerateToken(refreshTokenEntity.User);
            var newRefreshToken = GenerateRefreshToken();
            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = refreshTokenEntity.UserId
            };
            _dbContext.RefreshTokens.Add(newRefreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            return new RefreshResponseDto
            {
                AccessToken = newJwt,
                RefreshToken = newRefreshToken
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

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
