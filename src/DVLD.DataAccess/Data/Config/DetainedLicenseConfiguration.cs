using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class DetainedLicenseConfiguration : IEntityTypeConfiguration<DetainedLicense>
    {
        public void Configure(EntityTypeBuilder<DetainedLicense> builder)
        {
            builder.HasKey(dl => dl.DetainId);

            builder.Property(dl => dl.DetainDate)
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(dl => dl.FineFees)
                .HasPrecision(6, 2)
                .IsRequired();

            builder.Property(dl => dl.IsReleased)
                .HasColumnType("BIT")
                .IsRequired();

            builder.Property(dl => dl.ReleaseDate)
               .HasColumnType("DATETIME")
               .IsRequired(false);

            // Indexes
            builder.HasIndex(dl => dl.LicenseId);
            builder.HasIndex(dl => dl.CreatedByUserId);
            builder.HasIndex(dl => dl.ReleaseByUserId);
            builder.HasIndex(dl => dl.ReleaseApplicationId)
                .IsUnique().HasFilter("[ReleaseApplicationId] IS NOT NULL");
 
            // Relationships
            builder.HasOne(dl => dl.License)
                .WithMany(dl => dl.DetainedLicenses)
                .HasForeignKey(dl => dl.LicenseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dl => dl.CreatedByUser)
               .WithMany()
               .HasForeignKey(dl => dl.CreatedByUserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dl => dl.ReleaseByUser)
               .WithMany()
               .HasForeignKey(dl => dl.ReleaseByUserId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dl => dl.ReleaseApplication)
               .WithOne()
               .HasForeignKey<DetainedLicense>(dl => dl.ReleaseApplicationId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
