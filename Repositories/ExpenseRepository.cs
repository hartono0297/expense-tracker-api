using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories.Interfaces;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using ExpenseTracker.DTOs.ExpenseDtos;

namespace ExpenseTracker.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ExpenseRepository(AppDbContext context, IMapper mapper) => (_context, _mapper) = (context, mapper);

        public async Task<List<ExpenseDto>> GetPagedAsync(int user, int skip, int take, string? search = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Expenses
                .AsNoTracking()
                .Where(e => e.UserId == user)
                .OrderByDescending(e => e.CreatedAt)
                .ProjectTo<ExpenseDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrEmpty(search))
            {
                // Apply search filters in DB using simple searchable fields
                search = search.ToLower();
                query = query.Where(e =>
                    e.Title.ToLower().Contains(search) ||
                    (e.Note != null && e.Note.ToLower().Contains(search)) ||
                    e.CategoryName.ToLower().Contains(search) ||
                    e.Amount.ToString().Contains(search));
            }

            return await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(int userId, string? search = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Expenses
                .AsNoTracking()
                .Where(e => e.UserId == userId);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(e =>
                    e.Title.ToLower().Contains(search) ||
                    (e.Note != null && e.Note.ToLower().Contains(search)) ||
                    e.Category!.Name.ToLower().Contains(search) ||
                    e.Amount.ToString().Contains(search));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Expense> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Expenses.Include(e => e.Category).Include(e => e.User).FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<Expense> EditByIdAsync(int Id, Expense expense, CancellationToken cancellationToken = default)
        {
            var existingExpense = await GetByIdAsync(Id, cancellationToken);
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

        public async Task<Expense> AddAsync(Expense expen, CancellationToken cancellationToken = default)
        {
            var entity = (await _context.Expenses.AddAsync(expen, cancellationToken)).Entity;
            return entity;
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task<bool> CategoryExistsAsync(int categoryId, int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(
                    c => c.Id == categoryId && (c.UserId == null || c.UserId == userId) && c.IsActive == true, cancellationToken);
        }

        public async Task<bool> IsSameUserAsync(int ExpenseId, int userId, CancellationToken cancellationToken = default)
        {
            var expense = await _context.Expenses.FindAsync(new object[] { ExpenseId }, cancellationToken);

            if (expense == null)
            {
                throw new ArgumentException("Expense not found");
            }

            return expense.UserId == userId;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var expense = await GetByIdAsync(id, cancellationToken);

            if (expense == null)
            {
                throw new ArgumentException("Expense not found");
            }

            _context.Expenses.Remove(expense);
            await SaveAsync(cancellationToken);
        }

        // reports
        public async Task<List<Expense>> GetExpensesByUserAndMonthAsync(int userId, int month, int year, int skip, int take, CancellationToken cancellationToken = default)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .Where (e => e.UserId == userId &&
                        e.ExpenseDate.Month == month && 
                        e.ExpenseDate.Year == year)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountReportAsync(int userId, int month, int year, CancellationToken cancellationToken = default)
        {
            return await _context.Expenses.CountAsync(e => e.UserId == userId && e.ExpenseDate.Month == month && e.ExpenseDate.Year == year, cancellationToken);
        }
    }
}
