using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceTracker.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
