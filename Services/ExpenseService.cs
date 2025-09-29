using ExpenseTracker.Controllers;
using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Interfaces;
using AutoMapper;
using System.Threading;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ILogger<ExpenseService> _logger;
        private readonly IMapper _mapper;

        public ExpenseService(IExpenseRepository expenseRepository, ILogger<ExpenseService> logger, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<ExpenseDto>> GetExpensesAsync(int user, int page, int limit, string? search = null, CancellationToken cancellationToken = default)
        {
            var skip = (page - 1) * limit;
            var expenses = await _expenseRepository.GetPagedAsync(user, skip, limit, search, cancellationToken);
            var totalItems = await _expenseRepository.CountAsync(user, search, cancellationToken);

            var result = _mapper.Map<List<ExpenseDto>>(expenses);

            return new PaginatedResponse<ExpenseDto>(result, page, limit, totalItems);
        }

        public async Task<ExpenseDto> CreateExpenseAsync(ExpenseCreateDto dto, int userId, CancellationToken cancellationToken = default)
        {
            // validate user existence
            var userExists = await _expenseRepository.UserExistsAsync(userId, cancellationToken);
            if (!userExists)
            {
                _logger.LogWarning("User with ID {UserId} does not exist.", userId);
                throw new ArgumentException($"User with ID {userId} does not exist.");
            }

            // validate category
            var categoryExists = await _expenseRepository.CategoryExistsAsync(dto.CategoryId, userId, cancellationToken);
            if (!categoryExists)
            {
                _logger.LogWarning("Category with ID {CategoryId} does not exist.", dto.CategoryId);
                throw new ArgumentException($"Category with ID {dto.CategoryId} does not exist.");
            }

            var entity = _mapper.Map<Models.Expense>(dto);
            entity.UserId = userId;

            var createdEntity = await _expenseRepository.AddAsync(entity, cancellationToken);
            await _expenseRepository.SaveAsync(cancellationToken);
            var created = await _expenseRepository.GetByIdAsync(createdEntity.Id, cancellationToken);

            _logger.LogInformation("Expense created successfully: {ExpenseId} for user {UserId}", created.Id, created.UserId);

            return _mapper.Map<ExpenseDto>(created);

        }

        public async Task<ExpenseDto> UpdateExpenseAsync (int id, ExpenseUpdateDto dto, int userId, CancellationToken cancellationToken = default)
         {
            var isSameUser = await _expenseRepository.IsSameUserAsync(id, userId, cancellationToken);

            if (!isSameUser)
            {
                throw new ArgumentException($"Expense with ID {id} does not belong to user with ID {userId}.");
            }

            var categoryExists = await _expenseRepository.CategoryExistsAsync(dto.CategoryId, userId, cancellationToken);
            if (!categoryExists)
            {
                _logger.LogWarning("Category with ID {CategoryId} does not exist.", dto.CategoryId);
                throw new ArgumentException($"Category with ID {dto.CategoryId} does not exist.");
            }

            var entity = _mapper.Map<Models.Expense>(dto);
            entity.Id = id;
            entity.UserId = userId;

            await _expenseRepository.EditByIdAsync(id, entity, cancellationToken);
            await _expenseRepository.SaveAsync(cancellationToken);

            var updated = await _expenseRepository.GetByIdAsync(id, cancellationToken);

            return _mapper.Map<ExpenseDto>(updated);
         }

        public async Task<int> DeleteExpenseAsync (int id, int userId, CancellationToken cancellationToken = default)
        {
            var isSameUser = await _expenseRepository.IsSameUserAsync(id, userId, cancellationToken);

            if (!isSameUser)
            {
                throw new ArgumentException($"Expense with ID {id} does not belong to user with ID {userId}.");
            }
            await _expenseRepository.DeleteAsync(id, cancellationToken);
            await _expenseRepository.SaveAsync(cancellationToken);

            return id;
        }

        public async Task<ExpenseDto> GetExpenseByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, cancellationToken);
            if (expense == null || expense.UserId != userId)
                return null;
            return _mapper.Map<ExpenseDto>(expense);
        }


    }
}
