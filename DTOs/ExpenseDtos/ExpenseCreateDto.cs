namespace ExpenseTracker.DTOs.ExpenseDtos
{
    public class ExpenseCreateDto
    {
        //public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Note { get; set; }
    }
}
