using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.DTOs.ExpenseDtos
{
    public class ExpenseCreateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title must be at most 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "ExpenseDate is required.")]
        public DateTime ExpenseDate { get; set; }

        [StringLength(500, ErrorMessage = "Note must be at most 500 characters.")]
        public string? Note { get; set; }
    }
}
