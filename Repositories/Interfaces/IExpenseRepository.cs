using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using ExpenseTracker.DTOs.ExpenseDtos;
using System.Threading;

namespace ExpenseTracker.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task<List<ExpenseDto>> GetPagedAsync(int user, int skip, int take, string? search = null, CancellationToken cancellationToken = default);
        Task<int> CountAsync(int userId, string? search = null, CancellationToken cancellationToken = default);
        Task<Expense> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Expense> EditByIdAsync(int Id, Expense expense, CancellationToken cancellationToken = default);
        Task<Expense> AddAsync(Expense expense, CancellationToken cancellationToken = default);
        Task SaveAsync(CancellationToken cancellationToken = default);
        Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> CategoryExistsAsync(int categoryId, int userId, CancellationToken cancellationToken = default);
        Task<bool> IsSameUserAsync(int ExpenseId, int userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        //reports
        Task<List<Expense>> GetExpensesByUserAndMonthAsync(int userId, int month, int year, int skip, int take, CancellationToken cancellationToken = default);
        Task<int> CountReportAsync(int userId, int month, int year, CancellationToken cancellationToken = default);
    }
}
