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
            builder.Property(b => b.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // 2. Số tiền hạn mức (Precision 18,2)
            builder.Property(b => b.LimitAmount)
                .HasColumnType("decimal(18,2)")
                .HasPrecision(18, 2)
                .IsRequired();

            // 3. Thời gian áp dụng ngân sách
            builder.Property(b => b.StartDate)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(b => b.EndDate)
                .HasColumnType("datetime2")
                .IsRequired();

            // 4. Quan hệ với User
            builder.HasOne(b => b.User)
                .WithMany(u => u.Budgets)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 5. Quan hệ với Category
            builder.HasOne(b => b.Category)
                .WithMany(c => c.Budgets)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. Tối ưu hóa truy vấn
            builder.HasIndex(b => new { b.UserId, b.StartDate, b.EndDate })
                .HasDatabaseName("IX_Budget_User_DateRange");
        }
    }
}