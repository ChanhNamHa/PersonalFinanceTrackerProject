using System;

namespace PersonalFinanceTracker.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; } = null;

        public DateTime? UpdatedAt { get; set; } = null;
        public string? UpdatedBy { get; set; } = null;
    }
}
