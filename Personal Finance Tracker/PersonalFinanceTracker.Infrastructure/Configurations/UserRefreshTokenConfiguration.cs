using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Configurations
{
    public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.HasKey(urt => urt.Id);
            builder.Property(urt => urt.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(urt => urt.TokenHash)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(urt => urt.ExpiresAt)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(urt => urt.CreatedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(urt => urt.DeviceInfo)
                .HasMaxLength(250)
                .IsRequired(false);

            builder.Property(urt => urt.RevokedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            builder.Property(urt => urt.ReplacedByTokenHash)
                .HasMaxLength(256)
                .IsRequired(false);

            builder.HasOne(urt => urt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(urt => urt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
