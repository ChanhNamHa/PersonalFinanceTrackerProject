using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Infrastructure.Repositories;
using PersonalFinanceTracker.Infrastructure.Services;
using PersonalFinanceTracker.Application.Settings;

namespace PersonalFinanceTracker.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //DbContext
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Bind JwtOptions
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

            //Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Service
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IBudgetRepository, BudgetRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            //Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IBudgetRepository, BudgetRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            return services;
        }
    }
}