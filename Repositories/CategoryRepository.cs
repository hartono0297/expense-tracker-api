using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Category>> PagingCategoryAsync(int user, int skip, int take, string? search = null, CancellationToken cancellationToken = default)
        {
            // Include global categories (UserId == null) in results
            var query = _context.Categories
                .Where(c => c.UserId == null || c.UserId == user);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmed = search.Trim();
                query = query.Where(c => c.Name.Contains(trimmed));
            }

            var results = await query
                .OrderByDescending(c => c.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<int> CountAsync(int userId, string? search = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Categories
                .Where(c => c.UserId == null || c.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmed = search.Trim();
                query = query.Where(c => c.Name.Contains(trimmed));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<List<Category>> GetAllCategoriesAsync(int user, bool isActive = true, CancellationToken cancellationToken = default)
        {
            var query = _context.Categories.Where(c => c.UserId == null || c.UserId == user);

            if (isActive)
                query = query.Where(c => c.IsActive == true);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Category> AddCategoryAsync(Category cate, int user, CancellationToken cancellationToken = default)
        {            
            await _context.Categories.AddAsync(cate, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return cate;
        }

        public async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Category> EditCategoryByIdAsync(int id, Category category, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (existing == null)
                throw new ArgumentException("Category not found");

            // update allowed fields
            existing.Name = category.Name;
            existing.IsActive = category.IsActive;
            // keep UserId and Id intact

            _context.Categories.Update(existing);
            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task ToggleActiveCategoryAsync(int id, int user, CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == user, cancellationToken);
            if (category == null)
                throw new ArgumentException("Category not found");

            category.IsActive = !category.IsActive;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteCategoryByIdAsync(int id, int user, CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == user, cancellationToken);
                if (category == null)
                    throw new ArgumentException("Category not found");

                // Find or create a global "Uncategorized" default category
                var defaultCategory = await _context.Categories.FirstOrDefaultAsync(c => c.UserId == null && c.Name == "Uncategorized", cancellationToken);
                if (defaultCategory == null)
                {
                    defaultCategory = new Category { Name = "Uncategorized", UserId = null, IsActive = true };
                    await _context.Categories.AddAsync(defaultCategory, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                var expenses = await _context.Expenses.Where(e => e.CategoryId == id && e.UserId == user).ToListAsync(cancellationToken);
                if (expenses.Count > 0)
                {
                    foreach (var expense in expenses)
                    {
                        expense.CategoryId = defaultCategory.Id;
                        _context.Expenses.Update(expense);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            });
        }

        public async Task<bool> CategoryExistsAsync(string name, int user, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var normalized = name.Trim().ToLower();
            return await _context.Categories.AnyAsync(c => c.Name.ToLower() == normalized && c.UserId == user && (!excludeId.HasValue || c.Id != excludeId.Value), cancellationToken);
        }

        public async Task<bool> IdCategoryExistsAsync(int id, int user, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id && c.UserId == user, cancellationToken);
        }

    }
}
