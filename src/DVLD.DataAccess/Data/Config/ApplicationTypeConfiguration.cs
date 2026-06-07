using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class ApplicationTypeConfiguration : IEntityTypeConfiguration<ApplicationType>
    {
        public void Configure(EntityTypeBuilder<ApplicationType> builder)
        {
            builder.HasKey(at => at.ApplicationTypeId);

            builder.Property(at => at.Title)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(at => at.Fees)
               .HasPrecision(6, 2)
               .IsRequired();
        }
    }
}
