namespace ExpenseTracker.DTOs.Reports
{
    public class CategorySummaryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class MonthlyReportDto
    {
        public decimal Total { get; set; }
        public List<CategorySummaryDto> ByCategory { get; set; } = new();
    }
}
