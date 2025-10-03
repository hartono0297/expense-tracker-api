using ExpenseTracker.DTOs.Reports;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Security.Authentication;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        public readonly IReportService _reportService;
        public readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet("monthly")]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<MonthlyReportDto>>>> GetMonthlyReportAsync(int user, int month, int year, int page = 1, int limit = 5)
        {
                var data = await _reportService.GetMonthlyReportAsync(user, month, year, page, limit);

                return Ok(new ApiResponse<PaginatedResponse<MonthlyReportDto>>(data, "Success"));                
        }
    }
}
