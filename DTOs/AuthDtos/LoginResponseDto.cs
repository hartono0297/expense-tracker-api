namespace ExpenseTracker.DTOs.AuthDtos
{
    public class LoginResponseDto
    {        
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }        
        public string RefreshToken { get; set; }
    }
}
