using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // 1. Khóa chính
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // 2. Định danh và Bảo mật
            builder.Property(u => u.Username)
                .HasMaxLength(50)
                .IsRequired();
            builder.HasIndex(u => u.Username).IsUnique();
            builder.Property(u => u.Email)
                .HasMaxLength(150)
                .IsRequired();

            // Tạo Index Unique cho Email
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            // 3. Cấu hình Refresh Token
            builder.Property(u => u.RefreshToken)
                .HasMaxLength(500)
                .IsRequired(false); // Cho phép Null ban đầu

            builder.Property(u => u.RefreshTokenExpiryTime)
                .HasColumnType("datetime2")
                .IsRequired(false);

            // 4. Quan hệ
            // Cascade Delete: Khi xóa User, các Giao dịch và Ngân sách của họ sẽ bị xóa theo
            builder.HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Budgets)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}