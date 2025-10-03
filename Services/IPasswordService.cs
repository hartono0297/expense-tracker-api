namespace ExpenseTracker.Services
{
    public interface IPasswordService
    {
        (byte[] Hash, byte[] Salt) HashPassword(string password);
        bool VerifyPassword(string password, byte[] hash, byte[] salt);
    }
}
