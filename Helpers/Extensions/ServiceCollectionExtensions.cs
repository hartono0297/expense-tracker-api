using ExpenseTracker.Repositories.Interfaces;
using ExpenseTracker.Repositories;
using ExpenseTracker.Services.Interfaces;
using ExpenseTracker.Services;

namespace ExpenseTracker.Helpers.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            // Add more here...
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJWTService, JwtTokenService>();
            services.AddScoped<IReportService, ReportService>();
            // Add more here...
            return services;
        }
    }
}
