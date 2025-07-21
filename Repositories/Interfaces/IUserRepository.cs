using ExpenseTracker.DTOs.RegisterDtos;
using ExpenseTracker.DTOs.UserDtos;
using ExpenseTracker.Models;

namespace ExpenseTracker.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetByUserNameAsync (string username);
        Task CreateUserAsync(RegisterRequestDto regis);
        Task UpdateUserAsync(int id, UserUpdateDto user);
        Task<bool> UserExistsAsync(string username);
        //Task<bool> ValidateUserAsync(string username, string password);
    }
}
