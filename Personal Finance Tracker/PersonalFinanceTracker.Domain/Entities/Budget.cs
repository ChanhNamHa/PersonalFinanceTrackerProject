using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceTracker.Domain.Entities
{
    public class Budget
    {
        public Guid Id { get; set; }
        public decimal LimitAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
