using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
using System.Threading;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<PaginatedResponse<ExpenseDto>> GetExpensesAsync(int user, int page, int limit, string? search = null, CancellationToken cancellationToken = default);
        Task<ExpenseDto> CreateExpenseAsync(ExpenseCreateDto dto, int userId, CancellationToken cancellationToken = default);
        Task<ExpenseDto> UpdateExpenseAsync(int id, ExpenseUpdateDto dto, int userId, CancellationToken cancellationToken = default);
        Task<int> DeleteExpenseAsync (int id, int userId, CancellationToken cancellationToken = default);
        Task<ExpenseDto> GetExpenseByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    }
}
