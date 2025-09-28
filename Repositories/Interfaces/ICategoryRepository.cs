using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.Models;

namespace ExpenseTracker.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> PagingCategoryAsync(int user, int skip, int take, string? search = null, CancellationToken cancellationToken = default);
        Task<List<Category>> GetAllCategoriesAsync(int user, bool isActive = true, CancellationToken cancellationToken = default);
        Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Category> EditCategoryByIdAsync(int id, Category category, CancellationToken cancellationToken = default);
        Task<Category> AddCategoryAsync(Category category, int user, CancellationToken cancellationToken = default);
        Task ToggleActiveCategoryAsync(int id, int user, CancellationToken cancellationToken = default);
        Task<int> CountAsync(int user, string? search = null, CancellationToken cancellationToken = default);
        Task DeleteCategoryByIdAsync(int id, int user, CancellationToken cancellationToken = default);
        Task<bool> CategoryExistsAsync(string name, int user, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> IdCategoryExistsAsync (int id, int user, CancellationToken cancellationToken = default);
    }
}
