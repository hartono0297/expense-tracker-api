using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.Extensions;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
        public async Task<ActionResult<ApiResponse<PaginatedResponse<CategoryPagingDto>>>> GetCategories([FromQuery] int page = 1, [FromQuery] int limit = 5, [FromQuery] string? search = null, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<PaginatedResponse<CategoryPagingDto>>.Fail("User is not authorized"));

            var data = await _categoryService.PagingCategoriesAsync(userId, page, limit, search, cancellationToken);
            return Ok(new ApiResponse<PaginatedResponse<CategoryPagingDto>>(data, "Success"));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAllCategories([FromQuery] bool isActive = true, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<List<CategoryDto>>.Fail("User is not authorized"));

            var categories = await _categoryService.GetAllCategoriesAsync(userId, isActive, cancellationToken);
            return Ok(new ApiResponse<List<CategoryDto>>(categories, "Success"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryById(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<CategoryDto>.Fail("User is not authorized"));

            var category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
            if (category == null)
                return NotFound(ApiResponse<CategoryDto>.Fail("Category not found"));

            return Ok(new ApiResponse<CategoryDto>(category, "Success"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CategoryCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<CategoryDto>.Fail("User is not authorized"));

            var category = await _categoryService.CreateCategoryAsync(dto, userId, cancellationToken);
            // Return 201 with location header using action
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, new ApiResponse<CategoryDto>(category, "Category created"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<CategoryDto>.Fail("User is not authorized"));

            var update = await _categoryService.UpdateCategoryAsync(id, userId, dto, cancellationToken);
            return Ok(new ApiResponse<CategoryDto>(update, "Category updated"));
        }

        [HttpPut("active/{id}")]
        public async Task<ActionResult<ApiResponse<int>>> ToggleActiveCategory(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<int>.Fail("User is not authorized"));

            var toggleId = await _categoryService.ToggleActiveCategoryAsync(id, userId, cancellationToken);
            return Ok(new ApiResponse<int>(toggleId, "category active status toggled"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<int>>> DeleteCategory(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<int>.Fail("User is not authorized"));

            var deletedId = await _categoryService.DeleteCategoryAsync(id, userId, cancellationToken);
            return Ok(new ApiResponse<int>(deletedId, "category deleted"));
        }
    }
}
