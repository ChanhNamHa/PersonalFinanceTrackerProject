using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(t => t.Note)
                .HasMaxLength(500);

            builder.Property(t => t.TransactionDate)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Category)
                .WithMany() 
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.TransactionDate);
        }
    }
}