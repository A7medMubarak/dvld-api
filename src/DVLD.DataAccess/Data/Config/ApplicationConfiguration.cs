using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.HasKey(a => a.ApplicationId);

            builder.Property(a => a.ApplicationDate)
                .HasColumnType("DATETIME")
                .IsRequired();          

            builder.Property(a => a.ApplicationStatus)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(a => a.LastStatusDate)
              .HasColumnType("DATETIME")
              .IsRequired();

            builder.Property(lc => lc.PaidFees)
               .HasPrecision(6, 2)
               .IsRequired();

            // Indexes
            builder.HasIndex(a => a.ApplicantPersonId);
            builder.HasIndex(a => a.ApplicationTypeId);
            builder.HasIndex(a => a.CreatedByUserId);

            // Relationships
            builder.HasOne(a => a.ApplicantPerson)
                .WithMany()
                .HasForeignKey(a => a.ApplicantPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ApplicationType)
               .WithMany()
               .HasForeignKey(a => a.ApplicationTypeId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.CreatedByUser)
               .WithMany()
               .HasForeignKey(a => a.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
