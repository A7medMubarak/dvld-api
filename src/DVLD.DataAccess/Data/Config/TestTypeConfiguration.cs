using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class TestTypeConfiguration : IEntityTypeConfiguration<TestType>
    {
        public void Configure(EntityTypeBuilder<TestType> builder)
        {
            builder.HasKey(tt => tt.TestTypeId);

            builder.Property(tt => tt.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(tt => tt.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(tt => tt.Fees)
                .HasPrecision(6,2)
                .IsRequired();
        }
    }
}

