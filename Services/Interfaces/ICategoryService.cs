using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;

namespace ExpenseTracker.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginatedResponse<CategoryPagingDto>> PagingCategoriesAsync(int user, int page, int limit, string? search = null, CancellationToken cancellationToken = default);
        Task<List<CategoryDto>> GetAllCategoriesAsync(int user, bool isActive = true, CancellationToken cancellationToken = default);
        Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto category, int user, CancellationToken cancellationToken = default);
        Task<CategoryDto> UpdateCategoryAsync(int id, int user, CategoryUpdateDto category, CancellationToken cancellationToken = default);
        Task<int> ToggleActiveCategoryAsync(int id, int user, CancellationToken cancellationToken = default);
        Task<int> DeleteCategoryAsync(int id, int user, CancellationToken cancellationToken = default);
    }
}
