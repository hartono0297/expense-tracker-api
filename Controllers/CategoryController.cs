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
        public async Task<ActionResult<ApiResponse<PaginatedResponse<CategoryPagingDto>>>> GetCategoriesAsync([FromQuery] int page = 1, [FromQuery] int limit = 5, [FromQuery] string? search = null, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<PaginatedResponse<CategoryPagingDto>>.Fail("User is not authorized"));

            var data = await _categoryService.PagingCategoriesAsync(userId, page, limit, search, cancellationToken);
            return Ok(new ApiResponse<PaginatedResponse<CategoryPagingDto>>(data));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAllCategoriesAsync([FromQuery] bool isActive = true, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<List<CategoryDto>>.Fail("User is not authorized"));

            var categories = await _categoryService.GetAllCategoriesAsync(userId, isActive, cancellationToken);
            return Ok(new ApiResponse<List<CategoryDto>>(categories));
        }

        [HttpGet("{id}", Name = "GetCategoryById")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<CategoryDto>.Fail("User is not authorized"));

            var category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
            if (category == null)
                return NotFound(ApiResponse<CategoryDto>.Fail("Category not found"));

            return Ok(new ApiResponse<CategoryDto>(category));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategoryAsync([FromBody] CategoryCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<CategoryDto>.Fail("User is not authorized"));

            var category = await _categoryService.CreateCategoryAsync(dto, userId, cancellationToken);
            // Return 201 with location header
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = category.Id }, new ApiResponse<CategoryDto>(category, "Category created"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategoryAsync(int id, [FromBody] CategoryUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<CategoryDto>.Fail("User is not authorized"));

            var update = await _categoryService.UpdateCategoryAsync(id, userId, dto, cancellationToken);
            return Ok(new ApiResponse<CategoryDto>(update, "Category updated"));
        }

        [HttpPut("active/{id}")]
        public async Task<ActionResult<ApiResponse<int>>> ToggleActiveCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<int>.Fail("User is not authorized"));

            var toggleId = await _categoryService.ToggleActiveCategoryAsync(id, userId, cancellationToken);
            return Ok(new ApiResponse<int>(toggleId, "category active status toggled"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<int>>> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<int>.Fail("User is not authorized"));

            var deletedId = await _categoryService.DeleteCategoryAsync(id, userId, cancellationToken);
            return Ok(new ApiResponse<int>(deletedId, "category deleted"));
        }
    }
}
