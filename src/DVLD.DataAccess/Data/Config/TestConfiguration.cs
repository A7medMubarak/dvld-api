using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.HasKey(t => t.TestId);

            builder.Property(t => t.TestResult)
                .HasColumnType("BIT")
                .IsRequired();

            builder.Property(t => t.Notes)
                .HasMaxLength(500)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(t => t.TestAppointmentId).IsUnique();
            builder.HasIndex(t => t.CreatedByUserId);

            // Relationships
            builder.HasOne(t => t.TestAppointment)
                .WithOne(ta => ta.Test)
                .HasForeignKey<Test>(t => t.TestAppointmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}

