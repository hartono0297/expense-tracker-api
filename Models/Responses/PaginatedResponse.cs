namespace ExpenseTracker.Models.Responses
{
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public PaginatedResponse(List<T> data, int page, int limit, int totalItems)
        {
            Data = data;
            Page = page;
            Limit = limit;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(totalItems / (double)limit);
        }
    }
}
