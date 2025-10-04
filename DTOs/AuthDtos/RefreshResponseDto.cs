namespace ExpenseTracker.DTOs.AuthDtos
{
    public class RefreshResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}