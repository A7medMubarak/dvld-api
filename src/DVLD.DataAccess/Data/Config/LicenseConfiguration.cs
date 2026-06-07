using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class LicenseConfiguration : IEntityTypeConfiguration<License>
    {
        public void Configure(EntityTypeBuilder<License> builder)
        {
            builder.HasKey(l => l.LicenseId);

            builder.Property(l => l.IssueDate)
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(l => l.ExpirationDate)
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(l => l.Notes)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(l => l.PaidFees)
                .HasPrecision(6,2)
                .IsRequired();

            builder.Property(l => l.IsActive)
                .HasColumnType("BIT")
                .IsRequired();

            builder.Property(l => l.IssueReason)
                .HasConversion<byte>()
                .IsRequired();

            // Indexes
            builder.HasIndex(l => l.ApplicationId).IsUnique();
            builder.HasIndex(l => l.DriverId);
            builder.HasIndex(l => l.LicenseClassId);
            builder.HasIndex(l => l.CreatedByUserId);


            // Relationships
            builder.HasOne(l => l.Application)
                .WithOne(a => a.License)
                .HasForeignKey<License>(l => l.ApplicationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.LicenseClass)
                .WithMany()
                .HasForeignKey(l => l.LicenseClassId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Driver)
                .WithMany(d => d.Licenses)
                .HasForeignKey(l => l.DriverId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.CreatedByUser)
                .WithMany()
                .HasForeignKey(l => l.CreatedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }




}

