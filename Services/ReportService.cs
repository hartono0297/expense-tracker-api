using ExpenseTracker.DTOs.Reports;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Services.Interfaces;

namespace ExpenseTracker.Services
{
    public class ReportService : IReportService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ILogger<ReportService> _logger;

        public ReportService (IExpenseRepository expenseRepository, ILogger<ReportService> logger)
        {
            _expenseRepository = expenseRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<MonthlyReportDto>> GetMonthlyReportAsync(int userId, int month, int year, int page, int limit)
        {
            var skip = (page - 1) * limit;
            var expenses = await _expenseRepository.GetExpensesByUserAndMonthAsync(userId, month, year, skip, limit);
            var totalItems = await _expenseRepository.CountReportAsync(userId, month, year);

            var byCategory = expenses
                .GroupBy(e => e.Category.Name)
                .Select(g => new CategorySummaryDto { CategoryName = g.Key, TotalAmount = g.Sum(e => e.Amount) })
                .ToList();

            var final = new MonthlyReportDto
            {
                Total = byCategory.Sum(c => c.TotalAmount),
                ByCategory = byCategory
            };            

            return new PaginatedResponse<MonthlyReportDto>(new List<MonthlyReportDto> { final }, page, limit, totalItems);            
        }
    }
}
