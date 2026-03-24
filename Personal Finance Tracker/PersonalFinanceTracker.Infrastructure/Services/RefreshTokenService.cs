using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Application.Settings;
using PersonalFinanceTracker.Domain.Entities;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PersonalFinanceTracker.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IUnitOfWork _uow;

        public RefreshTokenService(IOptions<JwtOptions> jwtOptions, IUnitOfWork uow)
        {
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _uow = uow;
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            // Hash incoming token using SHA256 (same as AuthService)
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(refreshToken);
            var tokenHash = Convert.ToBase64String(sha.ComputeHash(bytes));

            var existing = await _uow.RefreshTokens.GetByTokenHashAsync(tokenHash);

            if (existing == null || existing.ExpiresAt <= DateTime.UtcNow || existing.RevokedAt != null)
                throw new Exception("Invalid or expired refresh token.");

            var user = await _uow.Users.GetByIdAsync(existing.UserId);
            if (user == null)
                throw new Exception("User not found for refresh token.");

            existing.RevokedAt = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            using var sha2 = System.Security.Cryptography.SHA256.Create();
            var newHash = Convert.ToBase64String(sha2.ComputeHash(Encoding.UTF8.GetBytes(newRefreshToken)));
            existing.ReplacedByTokenHash = newHash;

            var newTokenEntity = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                DeviceInfo = null
            };

            await _uow.RefreshTokens.AddAsync(newTokenEntity);
            _uow.RefreshTokens.Update(existing);
            await _uow.CompleteAsync();

            // Generate access token locally using JwtOptions
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

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return new AuthResponse(accessToken, newRefreshToken, user.Username);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
