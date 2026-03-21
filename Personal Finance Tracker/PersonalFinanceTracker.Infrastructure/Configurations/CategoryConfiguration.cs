using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker.Domain.Entities;
using PersonalFinanceTracker.Domain.Common;

namespace PersonalFinanceTracker.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // 1. Khóa chính
            builder.HasKey(c => c.Id);

            // 2. Cấu hình thuộc tính Name
            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            // 3. Cấu hình thuộc tính Type (Sử dụng string từ CategoryTypes)
            builder.Property(c => c.Type)
                .HasMaxLength(20)
                .IsRequired();

            // 4. Seed dữ liệu mẫu sử dụng hằng số class
            builder.HasData(
                new Category
                {
                    Id = 1,
                    Name = "Lương hàng tháng",
                    Type = CategoryTypes.Income
                },
                new Category
                {
                    Id = 2,
                    Name = "Ăn uống & Nhà hàng",
                    Type = CategoryTypes.Expense
                },
                new Category
                {
                    Id = 3,
                    Name = "Di chuyển & Xăng xe",
                    Type = CategoryTypes.Expense
                },
                new Category
                {
                    Id = 4,
                    Name = "Tiền thưởng Freelance",
                    Type = CategoryTypes.Income
                },
                new Category
                {
                    Id = 5,
                    Name = "Hóa đơn Điện & Nước",
                    Type = CategoryTypes.Expense
                }
            );
        }
    }
}