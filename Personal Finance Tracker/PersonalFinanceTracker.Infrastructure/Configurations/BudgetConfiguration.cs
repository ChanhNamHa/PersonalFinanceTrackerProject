using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Configurations
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            // 1. Khóa chính
            builder.HasKey(b => b.Id);

            // 2. Số tiền hạn mức (Precision 18,2)
            builder.Property(b => b.LimitAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            // 3. Thời gian áp dụng ngân sách
            builder.Property(b => b.StartDate).IsRequired();
            builder.Property(b => b.EndDate).IsRequired();

            // 4. Quan hệ với User
            builder.HasOne(b => b.User)
                .WithMany(u => u.Budgets)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 5. Quan hệ với Category
            builder.HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            // Không cho xóa Category nếu đang có một bản kế hoạch ngân sách gắn với nó.

            // 6. Tối ưu hóa: Thường xuyên lọc ngân sách theo User và thời gian
            builder.HasIndex(b => new { b.UserId, b.StartDate, b.EndDate });
        }
    }
}