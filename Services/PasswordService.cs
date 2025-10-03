using System.Security.Cryptography;

namespace ExpenseTracker.Services
{
    public class PasswordService : IPasswordService
    {
        public (byte[] Hash, byte[] Salt) HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return (hash, salt);
        }

        public bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computed = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            if (computed.Length != hash.Length) return false;
            var result = true;
            for (int i = 0; i < computed.Length; i++)
            {
                if (computed[i] != hash[i]) result = false;
            }
            return result;
        }
    }
}
