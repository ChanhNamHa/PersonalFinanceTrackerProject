using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            // 1. Khóa chính
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // 2. Số tiền giao dịch
            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .HasPrecision(18, 2)
                .IsRequired();

            // 3. Ghi chú
            builder.Property(t => t.Note)
                .HasMaxLength(500)
                .IsRequired(false); // Cho phép Null như trong sơ đồ (Allow Nulls check)

            // 4. Thời gian giao dịch
            builder.Property(t => t.TransactionDate)
                .HasColumnType("datetime2")
                .IsRequired();

            // 5. Thời gian tạo bản ghi (Hệ thống tự sinh)
            builder.Property(t => t.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            // 6. Quan hệ với User
            builder.HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 7. Quan hệ với Category
            builder.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 8. Index để tối ưu truy vấn báo cáo và lọc theo User
            builder.HasIndex(t => t.UserId)
                .HasDatabaseName("IX_Transaction_UserId");

            builder.HasIndex(t => t.TransactionDate)
                .HasDatabaseName("IX_Transaction_Date");

            // Index kết hợp thường dùng cho lịch sử giao dịch của 1 user theo thời gian
            builder.HasIndex(t => new { t.UserId, t.TransactionDate });
        }
    }
}