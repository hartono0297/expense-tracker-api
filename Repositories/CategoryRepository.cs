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

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> PagingCategoryAsync(int user, int skip, int take, string? search = null)
        {            
            var query = _context.Categories  
                .Where(e => e.UserId == user)                
                .OrderByDescending(e => e.Name); 

            var results = await query.ToListAsync();
           
            if (!string.IsNullOrEmpty(search))
            {
                results = results.Where(e => e.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            return results.Skip(skip).Take(take).ToList();
        }

        public async Task<int> CountAsync(int userId, string? search = null)
        {
            // 1. Base query: Filter only by User ID in the database.
            var query = _context.Categories                
                .Where(e => e.UserId == userId); // No search conditions here for DB

            // 2. Fetch ALL relevant user's expenses into memory.
            var results = await query.ToListAsync();

            // 3. Perform ALL search filtering in memory.
            if (!string.IsNullOrEmpty(search))
            {
                results = results.Where(e => e.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 4. Return the count of the fully filtered in-memory list.
            return results.Count;
        }

        public async Task<List<Category>> GetAllCategoriesAsync(int user, bool isActive = true)
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

        public async Task<Category> AddCategoryAsync(Category cate, int user)
        {
            cate.UserId = user;            
            _context.Categories.Add(cate);
            await _context.SaveChangesAsync();

            return cate;
        }

        public async Task<Category> GetCategoryByIdAsync (int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> EditCategoryByIdAsync (int id, Category category)
        {                                  
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task ToggleActiveCategoryAsync(int id, int user)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == user);
            category.IsActive = !category.IsActive;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryByIdAsync(int id, int user)
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
        
        public async Task<bool> CategoryExistsAsync (string name, int user, int? excludeId = null)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name && c.UserId == user && (!excludeId.HasValue || c.Id != excludeId.Value) );
        }
        
        public async Task<bool> IdCategoryExistsAsync (int id, int user)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id && c.UserId == user);
        }

    }
}
