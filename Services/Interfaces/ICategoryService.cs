using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;

namespace ExpenseTracker.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginatedResponse<CategoryPagingDto>> PagingCategoriesAsync(int user, int page, int limit, string? search = null);
        Task<List<CategoryDto>> GetAllCategoriesAsync(int user, bool isActive = true);
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto category, int user);
        Task<CategoryDto> UpdateCategoryAsync(int id, int user, CategoryUpdateDto category);
        Task<int> ToggleActiveCategoryAsync(int id, int user);
        Task<int> DeleteCategoryAsync(int id, int user);
    }
}
