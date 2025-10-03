using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        public async Task<User> GetUserByIdAsync (int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User> GetByUserNameAsync (string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        public async Task UpdateUserAsync (User user, CancellationToken cancellationToken = default)
        {           
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }   

        public async Task<bool> UserExistsAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<bool> UserIdExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

    }
}
