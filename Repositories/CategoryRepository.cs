using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Globalization;

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
            // Build the base query for the given user
            var query = _context.Categories
                .Where(c => c.UserId == user);

            // Apply search filter at database level if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmed = search.Trim();
                query = query.Where(c => c.Name.Contains(trimmed));
            }

            // Apply ordering and pagination in the database
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
                .Where(c => c.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var trimmed = search.Trim();
                query = query.Where(c => c.Name.Contains(trimmed));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<List<Category>> GetAllCategoriesAsync(int user, bool isActive = true, CancellationToken cancellationToken = default)
        {
            if (isActive == true)
            { 
            return await _context.Categories.Where(c => c.UserId == null || c.UserId == user && c.IsActive == isActive).ToListAsync();
            }
            else
            {
            return await _context.Categories.Where(c => c.UserId == null || c.UserId == user).ToListAsync();
            }
        }

        public async Task<Category> AddCategoryAsync(Category cate, int user, CancellationToken cancellationToken = default)
        {
            cate.UserId = user;            
            _context.Categories.Add(cate);
            await _context.SaveChangesAsync();

            return cate;
        }

        public async Task<Category> GetCategoryByIdAsync (int id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> EditCategoryByIdAsync (int id, Category category, CancellationToken cancellationToken = default)
        {                                  
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task ToggleActiveCategoryAsync(int id, int user, CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == user);
            category.IsActive = !category.IsActive;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryByIdAsync(int id, int user, CancellationToken cancellationToken = default)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == user);            

            var expenses = await _context.Expenses.Where(e => e.CategoryId == id && e.UserId == user).ToListAsync();
            
            if (expenses.Count > 0) { 
                foreach (var expense in expenses)
                {
                    expense.CategoryId = 1; // Assuming 9 is the default category ID for uncategorized expenses
                }
            }

            _context.Categories.Remove(category);
            
            await _context.SaveChangesAsync();
        }
        
        public async Task<bool> CategoryExistsAsync (string name, int user, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name && c.UserId == user && (!excludeId.HasValue || c.Id != excludeId.Value) );
        }
        
        public async Task<bool> IdCategoryExistsAsync (int id, int user, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id && c.UserId == user);
        }

    }
}
