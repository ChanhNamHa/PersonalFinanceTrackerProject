using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker.Domain.Entities;
using PersonalFinanceTracker.Domain.Common;
using System;

namespace PersonalFinanceTracker.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // 1. Khóa chính
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // 2. Cấu hình thuộc tính Name
            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            // 3. Cấu hình thuộc tính Type
            builder.Property(c => c.Type)
                .HasMaxLength(20)
                .IsRequired();

            // 4. Seed dữ liệu mẫu với GUID cố định
            builder.HasData(
                new Category
                {
                    Id = Guid.Parse("A1B2C3D4-E5F6-4A1B-8C9D-0E1F2A3B4C5D"),
                    Name = "Lương hàng tháng",
                    Type = CategoryTypes.Income
                },
                new Category
                {
                    Id = Guid.Parse("B2C3D4E5-F6A1-4B2C-9D0E-1F2A3B4C5D6E"),
                    Name = "Ăn uống & Nhà hàng",
                    Type = CategoryTypes.Expense
                },
                new Category
                {
                    Id = Guid.Parse("C3D4E5F6-A1B2-4C3D-0E1F-2A3B4C5D6E7F"),
                    Name = "Di chuyển & Xăng xe",
                    Type = CategoryTypes.Expense
                },
                new Category
                {
                    Id = Guid.Parse("D4E5F6A1-B2C3-4D4E-1F2A-3B4C5D6E7F8A"),
                    Name = "Tiền thưởng Freelance",
                    Type = CategoryTypes.Income
                },
                new Category
                {
                    Id = Guid.Parse("E5F6A1B2-C3D4-4E5F-2A3B-4C5D6E7F8A9B"),
                    Name = "Hóa đơn Điện & Nước",
                    Type = CategoryTypes.Expense
                }
            );
        }
    }
}