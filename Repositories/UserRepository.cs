using Azure.Core;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs.AuthDtos;
using ExpenseTracker.DTOs.RegisterDtos;
using ExpenseTracker.DTOs.UserDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ExpenseTracker.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;
        private readonly PasswordHasher<User> _hasher = new();

        public async Task<User> GetUserByIdAsync (int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByUserNameAsync (string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task CreateUserAsync(RegisterRequestDto regis)
        {
            // Hash and salt the password
            CreatePasswordHash(regis.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Create the user object
            var user = new User
            {
                Username = regis.Username,
                NickName = regis.NickName,
                Email = regis.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User", // Default role, can be changed later
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateUserAsync (int id, UserUpdateDto userUpdateDto)
        {           
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            if (!string.IsNullOrWhiteSpace(userUpdateDto.Email))
            {
                user.Email = userUpdateDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(userUpdateDto.Password))
            {
                // Hash and salt the password
                CreatePasswordHash(userUpdateDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            if (!string.IsNullOrWhiteSpace(userUpdateDto.NickName))
            {
                user.NickName = userUpdateDto.NickName;
            }

            //_context.Users.Update(user);
            await _context.SaveChangesAsync();
        }   

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
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
