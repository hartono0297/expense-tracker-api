
using ExpenseTracker.Models;

namespace ExpenseTracker.Services.Interfaces
{
    public interface IJWTService
    {
        string GenerateToken(User user);
    }
}
