using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class LocalDrivingLicenseApplicationConfiguration : IEntityTypeConfiguration<LocalDrivingLicenseApplication>
    {
        public void Configure(EntityTypeBuilder<LocalDrivingLicenseApplication> builder)
        {
            builder.HasKey(ldl => ldl.LocalDrivingLicenseApplicationId);

            // Indexes
            builder.HasIndex(ldl => ldl.ApplicationId).IsUnique();
            builder.HasIndex(ldl => ldl.LicenseClassId);

            // Relatioships
            builder.HasOne(ldl => ldl.Application)
                .WithOne(a => a.LocalDrivingLicenseApplication)
                .HasForeignKey<LocalDrivingLicenseApplication>(ldl => ldl.ApplicationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ldl => ldl.LicenseClass)
               .WithMany()
               .HasForeignKey(ldl => ldl.LicenseClassId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        }
    }







}

