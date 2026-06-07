using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserId);

            builder.Property(u => u.UserName)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.IsActive)
                .HasColumnType("BIT")
                .IsRequired();

            builder.HasIndex(u => u.PersonId).IsUnique();
            builder.HasIndex(u => u.UserName).IsUnique();

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

