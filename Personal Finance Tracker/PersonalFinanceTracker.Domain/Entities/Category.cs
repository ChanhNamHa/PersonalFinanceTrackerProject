using PersonalFinanceTracker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceTracker.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = CategoryTypes.Expense;
    }
}
