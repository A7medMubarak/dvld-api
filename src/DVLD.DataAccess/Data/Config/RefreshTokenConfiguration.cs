using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Token)
                .HasMaxLength(44)
                .IsRequired();

            builder.HasIndex(rt => rt.Token).IsUnique();

            builder.Property(rt => rt.ExpiresAt).IsRequired();
            builder.Property(rt => rt.CreatedAt).IsRequired();
            builder.Property(rt => rt.RevokedAt);
            builder.Property(rt => rt.ReplacedByToken).HasMaxLength(44);

            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
