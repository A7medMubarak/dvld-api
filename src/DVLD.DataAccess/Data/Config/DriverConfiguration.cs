using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config   
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.DriverId);

            builder.Property(d => d.CreatedDate)
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(d => d.PersonId)
                .IsRequired();

            builder.Property(d => d.CreatedByUserId)
                .IsRequired();

            // Indexes
            builder.HasIndex(d => d.PersonId).IsUnique();
            builder.HasIndex(d => d.CreatedByUserId);

            // Relationships
            builder.HasOne(d => d.Person)
                .WithOne(p => p.Driver)
                .HasForeignKey<Driver>(d => d.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.CreatedByUser)
                .WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }

}
