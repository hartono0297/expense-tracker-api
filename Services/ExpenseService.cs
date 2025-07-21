using ExpenseTracker.Controllers;
using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Interfaces;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        public readonly IExpenseRepository _expenseRepository;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseService(IExpenseRepository expenseRepository, ILogger<ExpenseController> logger)
        {
            _expenseRepository = expenseRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ExpenseDto>> GetExpensesAsync(int user, int page, int limit, string? search = null)
        {
            var skip = (page - 1) * limit;
            var expenses = await _expenseRepository.GetPagedAsync(user, skip, limit, search);
            var totalItems = await _expenseRepository.CountAsync(user, search);

            var expenseDtos = expenses.Select(e => new ExpenseDto
            {
                Id = e.Id,
                Title = e.Title,
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Note = e.Note,
                CategoryId = e.Category.Id,
                CategoryName = e.Category.Name,
                Username = e.User.Username
            }).ToList();

            return new PaginatedResponse<ExpenseDto>(expenseDtos, page, limit, totalItems);
        }

        public async Task<ExpenseDto> CreateExpenseAsync(Expense dto)
        {
            var userExists = await _expenseRepository.UserExistsAsync(dto.UserId);
            if (!userExists)
            {
                _logger.LogWarning("User with ID {UserId} does not exist.", dto.UserId);
                throw new ArgumentException($"User with ID {dto.UserId} does not exist.");
            }

            var categoryExists = await _expenseRepository.CategoryExistsAsync(dto.CategoryId);
            if (!categoryExists)
            {
                _logger.LogWarning("Category with ID {CategoryId} does not exist.", dto.CategoryId);
                throw new ArgumentException($"Category with ID {dto.CategoryId} does not exist.");
            }
            
            await _expenseRepository.AddAsync(dto);
            await _expenseRepository.SaveAsync();
            
            var created = await _expenseRepository.GetByIdAsync(dto.Id);

            _logger.LogInformation("Expense created successfully: {ExpenseId} for user {UserId}", created.Id, dto.UserId);

            return new ExpenseDto
            {
                Id = created.Id,
                Title = created.Title,
                Amount = created.Amount,
                ExpenseDate = created.ExpenseDate,
                Note = created.Note,
                CategoryName = created.Category.Name,
                Username = created.User.Username
            };

        }

        public async Task<ExpenseDto> UpdateExpenseAsync (int id, Expense dto)
        {
            var isSameUser = await _expenseRepository.IsSameUserAsync(id, dto.UserId);

            if (!isSameUser)
            {
                throw new ArgumentException($"Expense with ID {id} does not belong to user with ID {dto.UserId}.");
            }

            await _expenseRepository.EditByIdAsync(id, dto);
            await _expenseRepository.SaveAsync();

            var updated = await _expenseRepository.GetByIdAsync(id);

            return new ExpenseDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Amount = updated.Amount,
                ExpenseDate = updated.ExpenseDate,
                Note = updated.Note,
                CategoryName = updated.Category.Name,
                Username = updated.User.Username
            };
        }

        public async Task<int> DeleteExpenseAsync (int id, int userId)
        {
            var isSameUser = await _expenseRepository.IsSameUserAsync(id, userId);

            if (!isSameUser)
            {
                throw new ArgumentException($"Expense with ID {id} does not belong to user with ID {userId}.");
            }
            await _expenseRepository.DeleteAsync(id);
            await _expenseRepository.SaveAsync();

            return id;
        }


    }
}
