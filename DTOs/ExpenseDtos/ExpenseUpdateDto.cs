using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.DTOs.ExpenseDtos
{
    public class ExpenseUpdateDto
    {
        //[Range(1, int.MaxValue, ErrorMessage = "UserId must be a positive integer.")]
        //public int UserId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")]
        public int CategoryId { get; set; }

        [StringLength(100, ErrorMessage = "Title must be at most 100 characters.")]
        public string? Title { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "ExpenseDate is required.")]
        public DateTime ExpenseDate { get; set; }

        [StringLength(500, ErrorMessage = "Note must be at most 500 characters.")]
        public string? Note { get; set; }
    }
}
