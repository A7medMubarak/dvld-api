using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class LicenseClassConfiguration : IEntityTypeConfiguration<LicenseClass>
    {
        public void Configure(EntityTypeBuilder<LicenseClass> builder)
        {
            builder.HasKey(lc => lc.LicenseClassId);

            builder.Property(lc => lc.ClassName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(lc => lc.ClassDescription)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(lc => lc.MinimumAllowedAge)
                .HasColumnType("TINYINT")
                .IsRequired();

            builder.Property(lc => lc.DefaultValidityLength)
               .HasColumnType("TINYINT")
               .IsRequired();

            builder.Property(lc => lc.ClassFees)
                .HasPrecision(6, 2)
                .IsRequired();
        }
    }


}

