using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}