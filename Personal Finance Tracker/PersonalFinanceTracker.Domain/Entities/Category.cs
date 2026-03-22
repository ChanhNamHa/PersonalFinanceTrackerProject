using PersonalFinanceTracker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace PersonalFinanceTracker.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = CategoryTypes.Expense;
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
