using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.DTOs.RegisterDtos;
using ExpenseTracker.DTOs.UserDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IUserService
    {        
        Task<UserResponseDto> GetUserByIdAsync (int id);
        Task CreateUserAsync(RegisterRequestDto regis);
        Task UpdateUserAsync (int id, UserUpdateDto user);
    }
}
