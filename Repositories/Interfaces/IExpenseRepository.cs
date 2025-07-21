using ExpenseTracker.DTOs;
using ExpenseTracker.Models;

namespace ExpenseTracker.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task<List<Expense>> GetPagedAsync(int user, int skip, int take, string? search = null);
        Task<int> CountAsync(int userId, string? search = null);
        Task<Expense> GetByIdAsync(int id);
        Task<Expense> EditByIdAsync(int Id, Expense expense);
        Task AddAsync(Expense expense);
        Task SaveAsync();
        Task<bool> UserExistsAsync(int userId);
        Task<bool> CategoryExistsAsync(int categoryId);
        Task<bool> IsSameUserAsync(int ExpenseId, int userId);
        Task DeleteAsync(int id);

        //reports
        Task<List<Expense>> GetExpensesByUserAndMonthAsync(int userId, int month, int year, int skip, int take);
        Task<int> CountReportAsync(int userId, int month, int year);
    }
}
