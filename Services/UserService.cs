using AutoMapper;
using ExpenseTracker.Controllers;
using ExpenseTracker.DTOs.RegisterDtos;
using ExpenseTracker.DTOs.UserDtos;
using ExpenseTracker.Common.Exceptions;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Interfaces;
using System.Text.RegularExpressions;

namespace ExpenseTracker.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IMapper mapper, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
            _passwordService = passwordService;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found.", id);
                throw new NotFoundException($"User with ID {id} not found.");
            }

            var userResponse = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return userResponse;
        }

        public async Task<RegisterResponseDto> CreateUserAsync(RegisterRequestDto regis, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.UserExistsAsync(regis.Username, cancellationToken);

            if (existingUser == true)
            {
                throw new ConflictException("User already exists");
            }

            // Hash and salt the password using centralized service
            var (passwordHash, passwordSalt) = _passwordService.HashPassword(regis.Password);

            var entity = _mapper.Map<User>(regis);
            entity.PasswordHash = passwordHash;
            entity.PasswordSalt = passwordSalt;
            entity.Role = "User"; // Default role, can be changed later

            await _userRepository.CreateUserAsync(entity, cancellationToken);

            var user = await _userRepository.GetByUserNameAsync(entity.Username, cancellationToken);

            var mappedUser = _mapper.Map<RegisterResponseDto>(user);

            return mappedUser;
        }

        public async Task<UserResponseDto> UpdateUserAsync (int id, UserUpdateDto userUpdateDto, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id, cancellationToken);

            if (existingUser == null)
            {
                throw new NotFoundException("User id not exists");
            }

            if (!string.IsNullOrWhiteSpace(userUpdateDto.Email))
            {
                existingUser.Email = userUpdateDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(userUpdateDto.Password))
            {
                // Hash and salt the password
                var (passwordHash, passwordSalt) = _passwordService.HashPassword(userUpdateDto.Password);

                existingUser.PasswordHash = passwordHash;
                existingUser.PasswordSalt = passwordSalt;
            }

            if (!string.IsNullOrWhiteSpace(userUpdateDto.NickName))
            {
                existingUser.NickName = userUpdateDto.NickName;
            }

            await _userRepository.UpdateUserAsync(existingUser, cancellationToken);

            var mappedUser = _mapper.Map<UserResponseDto>(existingUser);

            return mappedUser;
        }
    }
}
