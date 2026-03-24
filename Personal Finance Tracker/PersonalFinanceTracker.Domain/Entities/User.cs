using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace PersonalFinanceTracker.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Refresh Tokens (multiple sessions/devices)
        public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();

        // Navigation properties
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
