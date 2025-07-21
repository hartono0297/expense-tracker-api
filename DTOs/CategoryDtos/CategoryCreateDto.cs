namespace ExpenseTracker.DTOs.CategoryDtos
{
    public class CategoryCreateDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; } = true; 
    }

}
