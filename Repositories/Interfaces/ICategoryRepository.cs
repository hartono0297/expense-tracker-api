using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.Models;

namespace ExpenseTracker.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> PagingCategoryAsync(int user, int skip, int take, string? search = null);
        Task<List<Category>> GetAllCategoriesAsync(int user, bool isActive = true);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> EditCategoryByIdAsync(int id, Category category);
        Task<Category> AddCategoryAsync(Category category, int user);
        Task ToggleActiveCategoryAsync(int id, int user);
        Task<int> CountAsync(int user, string? search = null);
        Task DeleteCategoryByIdAsync(int id, int user);
        Task<bool> CategoryExistsAsync(string name, int user, int? excludeId = null);
        Task<bool> IdCategoryExistsAsync (int id, int user);
    }
}
