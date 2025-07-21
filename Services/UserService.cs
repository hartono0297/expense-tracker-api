using ExpenseTracker.Controllers;
using ExpenseTracker.DTOs.RegisterDtos;
using ExpenseTracker.DTOs.UserDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Interfaces;

namespace ExpenseTracker.Services
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository;
        public readonly ILogger<UserController> _logger;
        
        public UserService(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found.", id);
                throw new ArgumentException($"User with ID {id} not found.");
            }

            var userResponse = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return userResponse;
        }

        public async Task CreateUserAsync(RegisterRequestDto regis)
        {
            var existingUser = await _userRepository.UserExistsAsync(regis.Username);

            if (existingUser == true)
            { 
                throw new ArgumentException("User already exists");
            }

            await _userRepository.CreateUserAsync(regis);
            return;
        }

        public async Task UpdateUserAsync (int id, UserUpdateDto userUpdateDto)
        {
            await _userRepository.UpdateUserAsync(id, userUpdateDto);
            return;
        }
    }
}
