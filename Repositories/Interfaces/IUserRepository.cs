using ExpenseTracker.DTOs.RegisterDtos;
using ExpenseTracker.DTOs.UserDtos;
using ExpenseTracker.Models;

namespace ExpenseTracker.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<User> GetByUserNameAsync (string username, CancellationToken cancellationToken = default);
        Task CreateUserAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> UserExistsAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> UserIdExistsAsync(int id, CancellationToken cancellationToken = default);
        //Task<bool> ValidateUserAsync(string username, string password);
    }
}
