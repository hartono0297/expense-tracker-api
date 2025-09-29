using ExpenseTracker.Data;
using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Services.Interfaces;
using ExpenseTracker.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(IExpenseService expenseService, ILogger<ExpenseController> logger)
        {
            _expenseService = expenseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ExpenseDto>>>> GetExpenses(int page = 1, int limit = 5, string? search = null, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<PaginatedResponse<ExpenseDto>>.Fail("User is not authorized"));

            var data = await _expenseService.GetExpensesAsync(userId, page, limit, search, cancellationToken);
            return Ok(new ApiResponse<PaginatedResponse<ExpenseDto>>(data, "Success"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ExpenseDto>>> GetExpenseById(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<ExpenseDto>.Fail("User is not authorized"));

            var expense = await _expenseService.GetExpenseByIdAsync(id, userId, cancellationToken);
            if (expense == null)
                return NotFound(ApiResponse<ExpenseDto>.Fail("Expense not found"));

            return Ok(new ApiResponse<ExpenseDto>(expense, "Success"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ExpenseDto>>> CreateExpense(ExpenseCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<ExpenseDto>.Fail("User is not authorized"));

            var data = await _expenseService.CreateExpenseAsync(dto, userId, cancellationToken);
            return CreatedAtAction(nameof(GetExpenseById), new { id = data.Id }, new ApiResponse<ExpenseDto>(data, "expense created"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ExpenseDto>>> UpdateExpense(int id, ExpenseUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<ExpenseDto>.Fail("User is not authorized"));

            var update = await _expenseService.UpdateExpenseAsync(id, dto, userId, cancellationToken);
            return Ok(new ApiResponse<ExpenseDto>(update, "expense updated"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<int>>> DeleteExpense(int id, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<int>.Fail("User is not authorized"));

            var deletedId = await _expenseService.DeleteExpenseAsync(id, userId, cancellationToken);
            return Ok(new ApiResponse<int>(deletedId, "expense deleted"));
        }

    }
}
