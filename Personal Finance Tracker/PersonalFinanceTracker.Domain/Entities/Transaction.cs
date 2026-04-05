using PersonalFinanceTracker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceTracker.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime TransactionDate { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
