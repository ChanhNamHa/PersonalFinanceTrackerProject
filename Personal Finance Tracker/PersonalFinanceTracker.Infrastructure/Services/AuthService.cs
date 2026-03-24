using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PersonalFinanceTracker.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _uow;

        public AuthService(IConfiguration config, IUnitOfWork uow)
        {
            _config = config;
            _uow = uow;
        }

        // --- LOGIC NGHIỆP VỤ ---

        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            // Kiểm tra email tồn tại thông qua Repository
            var existingUser = await _uow.Users.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("Email này đã được đăng ký hệ thống.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password)
            };

            await _uow.Users.AddAsync(user);
            await _uow.CompleteAsync(); // Commit vào DB

            return "Đăng ký tài khoản thành công.";
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _uow.Users.GetByEmailAsync(request.Email);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                throw new Exception("Tài khoản hoặc mật khẩu không chính xác.");

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            // Cập nhật Refresh Token vào User Entity
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _uow.CompleteAsync();

            return new AuthResponse(accessToken, refreshToken, user.Username);
        }

        // --- LOGIC HELPER (JWT & BCRYPT) ---

        public string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password, string hashedPassword)
            => BCrypt.Net.BCrypt.Verify(password, hashedPassword);

        public string GenerateAccessToken(User user)
        {
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing in config");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(3000),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}