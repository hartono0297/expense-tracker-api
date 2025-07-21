using ExpenseTracker.DTOs.AuthDtos;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto login);
    }
}
