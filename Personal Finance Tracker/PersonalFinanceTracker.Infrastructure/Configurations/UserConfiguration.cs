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

            // 2. Định danh và Bảo mật
            builder.Property(u => u.Username)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(150)
                .IsRequired();

            // Tạo Index Unique cho Email để không có 2 tài khoản trùng email
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            // 3. Cấu hình Refresh Token (Dùng cho cơ chế gia hạn JWT)
            builder.Property(u => u.RefreshToken)
                .HasMaxLength(500); // Lưu trữ chuỗi token ngẫu nhiên

            builder.Property(u => u.RefreshTokenExpiryTime)
                .IsRequired(false);

            // 4. Quan hệ (Đã được định nghĩa từ phía Transaction/Budget nhưng khai báo thêm để tường minh)
            builder.HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}