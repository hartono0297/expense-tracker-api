namespace ExpenseTracker.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Expense> Expenses { get; set; }

        public int? UserId { get; set; } // nullable

        public bool IsActive { get; set; } = true;
    }
}
