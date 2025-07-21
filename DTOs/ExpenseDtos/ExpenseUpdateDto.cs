namespace ExpenseTracker.DTOs.ExpenseDtos
{
    public class ExpenseUpdateDto
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string? Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Note { get; set; }
    }
}
