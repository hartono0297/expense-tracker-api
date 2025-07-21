using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet("paging")]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<CategoryPagingDto>>>> GetCategoriesAsync(int page = 1, int limit = 5, string? search = null)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var data = await _categoryService.PagingCategoriesAsync(userId, page, limit, search);

                return Ok(new ApiResponse<PaginatedResponse<CategoryPagingDto>>(data));
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while fetching categories");
                return BadRequest(new ApiResponse<PaginatedResponse<CategoryPagingDto>>("Database error occurred"));
            }
        }

        [HttpGet]
        public async Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync(bool isActive = true)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                
                var categories = await _categoryService.GetAllCategoriesAsync(userId, isActive);
                return new ApiResponse<List<CategoryDto>>(categories);
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while fetching categories");
                return new ApiResponse<List<CategoryDto>>("Database error occurred");
            }   
        }
        
        [HttpPost]
        public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync (CategoryCreateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var category = await _categoryService.CreateCategoryAsync(dto, userId);
                return new ApiResponse<CategoryDto>(category, "category created");
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while creating category");
                return new ApiResponse<CategoryDto>("Database error occurred");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while creating category");
                return new ApiResponse<CategoryDto>(ex.Message);
            }
        }
       
        [HttpPut("{id}")]
        public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync (int id, CategoryUpdateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var update = await _categoryService.UpdateCategoryAsync(id, userId, dto);
                return ApiResponse<CategoryDto>.SuccessResponse(update, "category updated");
                //return new ApiResponse<CategoryDto>(update, "category updated");
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while creating category");
                //return new ApiResponse<CategoryDto>("Database error occurred");
                return ApiResponse<CategoryDto>.Fail("Database error occurred while updating category");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while updating category");
                //return new ApiResponse<CategoryDto>(ex.Message);
                return ApiResponse<CategoryDto>.Fail(ex.Message);
            }
        }
        
        [HttpPut("active/{id}")]
        public async Task<ApiResponse<int>> ToggleActiveCategoryAsync (int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                await _categoryService.ToggleActiveCategoryAsync(id, userId);
                return ApiResponse<int>.SuccessResponse(id, "category deleted");
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while deleting category");
                return ApiResponse<int>.Fail("Database error occurred while deleting category");
            }
            catch (ArgumentException ex)
            { 
                _logger.LogError(ex, "Error occurred while toggling category");
                return ApiResponse<int>.Fail(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<int>> DeleteCategoryAsync(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                await _categoryService.DeleteCategoryAsync(id, userId);
                return ApiResponse<int>.SuccessResponse(id, "category deleted");
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while deleting category");
                return ApiResponse<int>.Fail("Database error occurred while deleting category");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category");
                return ApiResponse<int>.Fail(ex.Message);
            }
        }
    }
}
