using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface IAuthService
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string hashedPassword);
        public string GenerateAccessToken(User user);
        public string GenerateRefreshToken();
        public string HashRefreshToken(string refreshToken);

        public Task<string> RegisterAsync(RegisterRequest request);
        public Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}