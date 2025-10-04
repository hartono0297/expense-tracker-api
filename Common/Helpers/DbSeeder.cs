using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Common.Helpers
{
    public static class DbSeeder
    {
        public static async Task SeedDatabaseAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!await db.Categories.AnyAsync())
            {
                await db.Categories.AddRangeAsync(
                    new Category { Name = "Uncategorized", IsActive = true, UserId = null },
                    new Category { Name = "Food", IsActive = true, UserId = null },
                    new Category { Name = "Transport", IsActive = true, UserId = null },
                    new Category { Name = "Utilities", IsActive = true, UserId = null }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
