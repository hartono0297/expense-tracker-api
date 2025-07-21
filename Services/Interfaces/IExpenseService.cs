using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<PaginatedResponse<ExpenseDto>> GetExpensesAsync(int user, int page, int limit, string? search = null);
        Task<ExpenseDto> CreateExpenseAsync(Expense expense);
        Task<ExpenseDto> UpdateExpenseAsync(int id, Expense expense);
        Task<int> DeleteExpenseAsync (int id, int userId);
    }
}
