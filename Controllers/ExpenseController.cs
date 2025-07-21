using ExpenseTracker.Data;
using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ExpenseDto>>>> GetExpenses(int page = 1, int limit = 5, string? search = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var data = await _expenseService.GetExpensesAsync(userId, page, limit, search);
            return Ok(new ApiResponse<PaginatedResponse<ExpenseDto>>(data));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ExpenseDto>>> CreateExpense(ExpenseCreateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var expense = new Expense
                {
                    Title = dto.Title,
                    Amount = dto.Amount,
                    ExpenseDate = dto.ExpenseDate,
                    Note = dto.Note,
                    CategoryId = dto.CategoryId,
                    UserId = userId
                };                       
                //throw new Exception("Test crash");
                var data = await _expenseService.CreateExpenseAsync(expense);                
                return Ok(new ApiResponse<ExpenseDto>(data, "expense created"));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogWarning(dbEx, "Database error occurred while creating expense");
                return BadRequest(new ApiResponse<ExpenseDto>("Database error occured"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
                return BadRequest(new ApiResponse<ExpenseDto>(ex.Message));
            }        
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ExpenseDto>>> UpdateExpense(int id, ExpenseUpdateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var expense = new Expense
                {
                    Id = id,
                    Title = dto.Title,
                    Amount = dto.Amount,
                    ExpenseDate = dto.ExpenseDate,
                    Note = dto.Note,
                    CategoryId = dto.CategoryId,
                    UserId = userId
                };
            
                var update = await _expenseService.UpdateExpenseAsync(id, expense);
                return Ok(new ApiResponse<ExpenseDto>(update, "expense updated"));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogWarning(dbEx, "Database error occurred while updating expense for user {UserId}", dto.UserId);
                return BadRequest(new ApiResponse<ExpenseDto>("Database error occured"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new ApiResponse<ExpenseDto>(ex.Message));
            }       
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<int>>> DeleteExpense(int id)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || !int.TryParse(claim.Value, out var userId))
                return Unauthorized(new ApiResponse<int>("User is not authorized"));

            try
            {
                await _expenseService.DeleteExpenseAsync(id, userId);
                return NoContent(); // or Ok(new ApiResponse<int>(id, "Expense deleted"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
                return NotFound(new ApiResponse<int>(ex.Message));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogWarning(dbEx, "Database error occurred while deleting expense ID {ExpenseId}", id);
                return BadRequest(new ApiResponse<int>("Database error occurred"));
            }
        }

    }
}
