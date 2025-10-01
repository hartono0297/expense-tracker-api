using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.DTOs.RegisterDtos;
using ExpenseTracker.DTOs.UserDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IUserService
    {        
        Task<UserResponseDto> GetUserByIdAsync (int id, CancellationToken cancellationToken = default);
        Task<RegisterResponseDto> CreateUserAsync(RegisterRequestDto regis, CancellationToken cancellationToken = default);
        Task<UserResponseDto> UpdateUserAsync (int id, UserUpdateDto user, CancellationToken cancellationToken = default);
    }
}
