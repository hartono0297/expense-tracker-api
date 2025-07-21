namespace ExpenseTracker.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(T data, string message = "")
        {
            Success = true;
            Message = message;
            Data = data;
        }

        public ApiResponse(string message)
        {
            Success = false;
            Message = message;            
        }

        // 🔹 Static helper for success
        public static ApiResponse<T> SuccessResponse(T data, string message = "") =>
            new ApiResponse<T>(data, message);

        // 🔹 Static helper for failure
        public static ApiResponse<T> Fail(string message) =>
            new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default
            };


    }

}
