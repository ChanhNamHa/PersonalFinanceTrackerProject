using System;

namespace PersonalFinanceTracker.Domain.Entities
{
    public class UserRefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty; // store hash of refresh token
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DeviceInfo { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByTokenHash { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
