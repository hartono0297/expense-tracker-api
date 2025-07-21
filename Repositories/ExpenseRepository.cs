using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories.Interfaces;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _context;
        public ExpenseRepository(AppDbContext context) => _context = context;

        public async Task<List<Expense>> GetPagedAsync(int user, int skip, int take, string? search = null)
        {
            // 1. Base query: Filter only by User ID in the database.
            var query = _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.User)
                .Where(e => e.UserId == user)
                .OrderByDescending(e => e.CreatedAt); // Still good to order in DB for consistency

            // 2. Fetch ALL relevant user's expenses into memory.
            //    No 'search' conditions are applied to the DB query here.
            var results = await query.ToListAsync();

            // 3. Perform ALL search filtering in memory on the 'results' list.
            if (!string.IsNullOrEmpty(search))
            {
                var culture = new CultureInfo("id-ID"); // Ensure correct culture

                results = results.Where(e =>
                    // Apply all your search conditions here, including the date formatting
                    e.Amount.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Note.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Amount.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Category.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    // This is the key line that will catch "Juli" for dates
                    e.ExpenseDate.ToString("dd MMMM yyyy", culture) // Use "dd MMMM yyyy" or similar if you fixed it
                        .Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // 4. Apply pagination to the fully filtered in-memory list.
            return results.Skip(skip).Take(take).ToList();
        }

        public async Task<int> CountAsync(int userId, string? search = null)
        {
            // 1. Base query: Filter only by User ID in the database.
            var query = _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId); // No search conditions here for DB

            // 2. Fetch ALL relevant user's expenses into memory.
            var results = await query.ToListAsync();

            // 3. Perform ALL search filtering in memory.
            if (!string.IsNullOrEmpty(search))
            {
                var culture = new CultureInfo("id-ID");

                results = results.Where(e =>
                    e.Amount.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Note.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Amount.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Category.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.ExpenseDate.ToString("dd MMMM yyyy", culture) // Use "dd MMMM yyyy" or similar if you fixed it
                        .Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // 4. Return the count of the fully filtered in-memory list.
            return results.Count;
        }

        public async Task<Expense> GetByIdAsync(int id)
        {
            return await _context.Expenses.Include(e => e.Category).Include(e => e.User).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Expense> EditByIdAsync(int Id, Expense expense)
        {
            var existingExpense = await GetByIdAsync(Id);
            if (existingExpense == null)
            {
                throw new ArgumentException("Expense not found");
            }
            existingExpense.Title = expense.Title;
            existingExpense.Amount = expense.Amount;
            existingExpense.ExpenseDate = expense.ExpenseDate;
            existingExpense.Note = expense.Note;
            existingExpense.CategoryId = expense.CategoryId;
            _context.Expenses.Update(existingExpense);
            return existingExpense;
        }

        public async Task AddAsync(Expense expen)
        {
            await _context.Expenses.AddAsync(expen);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId);
        }

        public async Task<bool> IsSameUserAsync(int ExpenseId, int userId)
        {
            var expense = await _context.Expenses.FindAsync(ExpenseId);

            if (expense == null)
            {
                throw new ArgumentException("Expense not found");
            }

            if(expense.UserId != userId)
            {
                return false;
            }

            return true;
        }

        public async Task DeleteAsync (int id)
        {
            var expense = await GetByIdAsync(id);

            if (expense == null)
            {
                throw new ArgumentException("Expense not found");
            }

            _context.Expenses.Remove(expense);
            await SaveAsync();
        }

        // reports
        public async Task<List<Expense>> GetExpensesByUserAndMonthAsync(int userId, int month, int year, int skip, int take)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .Where (e => e.UserId == userId &&
                        e.ExpenseDate.Month == month && 
                        e.ExpenseDate.Year == year)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountReportAsync(int userId, int month, int year)
        {
            return await _context.Expenses.CountAsync(e => e.UserId == userId && e.ExpenseDate.Month == month && e.ExpenseDate.Year == year);
        }
    }
}
