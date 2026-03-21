using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace PersonalFinanceTracker.API.Extensions
{
    public static class Extensions
    {
        // Infrastructure/DependencyInjection.cs hoặc API/Extensions/ServiceExtensions.cs
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // ... cấu hình như ở phần trước
                    };
                });

            return services;
        }
    }
}
