using ExpenseTracker.DTOs.Reports;
using ExpenseTracker.Models.Responses;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IReportService
    {
        Task<PaginatedResponse<MonthlyReportDto>> GetMonthlyReportAsync(int userId, int month, int year, int page, int limit);
    }
}
