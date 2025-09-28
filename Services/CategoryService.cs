using AutoMapper;
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
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<CategoryPagingDto>> PagingCategoriesAsync(int user, int page, int limit, string? search = null, CancellationToken cancellationToken = default)
        {
            var skip = (page - 1) * limit;
            var categories = await _categoryRepository.PagingCategoryAsync(user, skip, limit, search, cancellationToken);
            var totalItems = await _categoryRepository.CountAsync(user, search, cancellationToken);                        

            var categoryDtos = categories.Select(e => new CategoryPagingDto
            {
                Id = e.Id,
                Name = e.Name,
                IsActive = e.IsActive
            }).ToList();

            return new PaginatedResponse<CategoryPagingDto>(categoryDtos, page, limit, totalItems);
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(int user, bool isActive = true, CancellationToken cancellationToken = default)
        {            
            var categories = await _categoryRepository.GetAllCategoriesAsync(user, isActive, cancellationToken);
                        
            return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync (int id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id, cancellationToken);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync (CategoryCreateDto cat, int user, CancellationToken cancellationToken = default)
        {   
            var existingCategory = await _categoryRepository.CategoryExistsAsync(cat.Name, user, excludeId :null, cancellationToken);
            
            if (existingCategory)
            {
                throw new ArgumentException("Category already exists");
            }

            var entity = _mapper.Map<Models.Category>(cat);
            entity.UserId = user;

            var category = await _categoryRepository.AddCategoryAsync(entity, user, cancellationToken);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, int user, CategoryUpdateDto categoryDto, CancellationToken cancellationToken = default)
        {
            if (categoryDto.Name == null || categoryDto.Name == "")
            {
                throw new ArgumentException("Category name cannot be empty");
            }

            var existingIdCategory = await _categoryRepository.IdCategoryExistsAsync(id, user, cancellationToken);
            if (!existingIdCategory)
            {
                throw new ArgumentException("ID Category not exists");
            }

            var isDuplicateName = await _categoryRepository.CategoryExistsAsync(categoryDto.Name, user, excludeId: id, cancellationToken);
            if (isDuplicateName)
            {
                throw new ArgumentException("Category name already used by another category");
            }

            var entity = _mapper.Map<Models.Category>(categoryDto);
            entity.UserId = user;
            entity.Id = id;

            var category = await _categoryRepository.EditCategoryByIdAsync(id, entity, cancellationToken);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<int> ToggleActiveCategoryAsync (int id, int user, CancellationToken cancellationToken = default)
        {
            //var existingIdCategory = await _categoryRepository.IdCategoryExistsAsync(id);
            
            //if (!existingIdCategory)
            //{
            //    throw new ArgumentException("ID Category not exists");
            //}

            await _categoryRepository.ToggleActiveCategoryAsync(id, user, cancellationToken);

            _logger.LogInformation("Toggle Active category with ID {Id}", id);

            return id;
        }
        public async Task<int> DeleteCategoryAsync(int id, int user, CancellationToken cancellationToken = default)
        {
            var existingIdCategory = await _categoryRepository.IdCategoryExistsAsync(id, user, cancellationToken);
            if (!existingIdCategory)
            {
                throw new ArgumentException("ID Category not exists");
            }

            await _categoryRepository.DeleteCategoryByIdAsync(id, user, cancellationToken);

            return id;
        }

    }
}
