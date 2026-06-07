using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class InternationalLicenseConfiguration : IEntityTypeConfiguration<InternationalLicense>
    {
        public void Configure(EntityTypeBuilder<InternationalLicense> builder)
        {
            builder.HasKey(i => i.InternationalLicenseId);

            builder.Property(i => i.IssueDate)
               .HasColumnType("DATETIME")
               .IsRequired();

            builder.Property(i => i.ExpirationDate)
               .HasColumnType("DATETIME")
               .IsRequired();

            builder.Property(i => i.IsActive)
               .HasColumnType("BIT")
               .IsRequired();

            // Indexes
            builder.HasIndex(i => i.ApplicationId).IsUnique();
            builder.HasIndex(i => i.DriverId);
            builder.HasIndex(i => i.IssuedUsingLocalLicenseId);
            builder.HasIndex(i => i.CreatedByUserId);

            // Relationships
            builder.HasOne(i => i.Application)
                .WithOne(a => a.InternationalLicense)
                .HasForeignKey<InternationalLicense>(i => i.ApplicationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Driver)
               .WithMany()
               .HasForeignKey(i => i.DriverId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.IssuedUsingLocalLicense)
                .WithMany()
                .HasForeignKey(i => i.IssuedUsingLocalLicenseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.CreatedByUser)
              .WithMany()           
              .HasForeignKey(i => i.CreatedByUserId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);




        }
    }





}

