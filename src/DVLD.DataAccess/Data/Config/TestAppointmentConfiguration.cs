using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class TestAppointmentConfiguration : IEntityTypeConfiguration<TestAppointment>
    {
        public void Configure(EntityTypeBuilder<TestAppointment> builder)
        {
            builder.HasKey(ta => ta.TestAppointmentId);

            builder.Property(ta => ta.AppointmentDate)
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(ta => ta.PaidFees)
                .HasPrecision(6, 2)
                .IsRequired();

            builder.Property(tt => tt.IsLocked)
                .HasColumnType("BIT")
                .IsRequired();

            // Indexes
            builder.HasIndex(ta => ta.TestTypeId);
            builder.HasIndex(ta => ta.LocalDrivingLicenseApplicationId);
            builder.HasIndex(ta => ta.CreatedByUserId);
            builder.HasIndex(ta => ta.RetakeTestApplicationId)
                .IsUnique().HasFilter("[RetakeTestApplicationId] IS NOT NULL");

            // Relationships
            builder.HasOne(ta => ta.TestType)
                .WithMany()
                .HasForeignKey(ta => ta.TestTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ta => ta.LocalDrivingLicenseApplication)
                .WithMany(ldl => ldl.TestAppointments)
                .HasForeignKey(ta => ta.LocalDrivingLicenseApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ta => ta.CreatedByUser)
               .WithMany()
               .HasForeignKey(ta => ta.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ta => ta.RetakeTestApplication)
               .WithOne()
               .HasForeignKey<TestAppointment>(ta => ta.RetakeTestApplicationId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}


