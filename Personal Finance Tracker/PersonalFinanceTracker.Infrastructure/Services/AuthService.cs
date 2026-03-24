using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Application.Settings;
using PersonalFinanceTracker.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PersonalFinanceTracker.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IUnitOfWork _uow;

        public AuthService(IOptions<JwtOptions> jwtOptions, IUnitOfWork uow)
        {
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
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

            // Hash refresh token before storing
            var refreshTokenHash = HashRefreshToken(refreshToken);

            var refreshTokenEntity = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                DeviceInfo = null
            };

            await _uow.RefreshTokens.AddAsync(refreshTokenEntity);
            await _uow.CompleteAsync();

            return new AuthResponse(accessToken, refreshToken, user.Username);
        }

        // --- LOGIC HELPER (JWT & BCRYPT) ---

        public string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password, string hashedPassword)
            => BCrypt.Net.BCrypt.Verify(password, hashedPassword);

        public string HashRefreshToken(string refreshToken)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(refreshToken);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public string GenerateAccessToken(User user)
        {
            if (string.IsNullOrEmpty(_jwtOptions.Key))
                throw new InvalidOperationException("JWT Key is missing in configuration.");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(Math.Max(1, _jwtOptions.AccessTokenExpirationMinutes));

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expires,
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