using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Repositories;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Interfaces;
using System.Formats.Asn1;

namespace ExpenseTracker.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<CategoryPagingDto>> PagingCategoriesAsync(int user, int page, int limit, string? search = null)
        {
            var skip = (page - 1) * limit;
            var categories = await _categoryRepository.PagingCategoryAsync(user, skip, limit, search);
            var totalItems = await _categoryRepository.CountAsync(user, search);

            var categoryDtos = categories.Select(e => new CategoryPagingDto
            {
                Id = e.Id,
                Name = e.Name,
                IsActive = e.IsActive
            }).ToList();

            return new PaginatedResponse<CategoryPagingDto>(categoryDtos, page, limit, totalItems);
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(int user, bool isActive = true)
        {            
            var categories = await _categoryRepository.GetAllCategoriesAsync(user, isActive);
           
            return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync (int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync (CategoryCreateDto cat, int user)
        {   
            var existingCategory = await _categoryRepository.CategoryExistsAsync(cat.Name, user);
            
            if (existingCategory)
            {
                throw new ArgumentException("Category already exists");
            }

            var cate = new Category
            {
                Name = cat.Name,        
                IsActive = cat.IsActive
            };

            var category = await _categoryRepository.AddCategoryAsync(cate, user);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, int user, CategoryUpdateDto categoryDto)
        {
            if (categoryDto.Name == null || categoryDto.Name == "")
            {
                throw new ArgumentException("Category name cannot be empty");
            }

            var existingIdCategory = await _categoryRepository.IdCategoryExistsAsync(id, user);
            if (!existingIdCategory)
            {
                throw new ArgumentException("ID Category not exists");
            }

            var isDuplicateName = await _categoryRepository.CategoryExistsAsync(categoryDto.Name, user, excludeId: id);
            if (isDuplicateName)
            {
                throw new ArgumentException("Category name already used by another category");
            }

            var cate = new Category
            {
                Id = id,
                Name = categoryDto.Name,
                UserId = user
            };

            var category = await _categoryRepository.EditCategoryByIdAsync(id,cate);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,                
            };
        }

        public async Task<int> ToggleActiveCategoryAsync (int id, int user)
        {
            //var existingIdCategory = await _categoryRepository.IdCategoryExistsAsync(id);
            
            //if (!existingIdCategory)
            //{
            //    throw new ArgumentException("ID Category not exists");
            //}

            await _categoryRepository.ToggleActiveCategoryAsync(id, user);

            _logger.LogInformation("Toggle Active category with ID {Id}", id);

            return id;
        }
        public async Task<int> DeleteCategoryAsync(int id, int user)
        {
            var existingIdCategory = await _categoryRepository.IdCategoryExistsAsync(id, user);
            if (!existingIdCategory)
            {
                throw new ArgumentException("ID Category not exists");
            }

            await _categoryRepository.DeleteCategoryByIdAsync(id, user);

            return id;
        }

    }
}
