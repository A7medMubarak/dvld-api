using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVLD.DataAccess.Data.Config
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(p => p.PersonId);
           
            builder.Property(p => p.NationalNo)
                .HasMaxLength(20)               
                .IsRequired();

            builder.Property(p => p.FirstName)
               .HasMaxLength(20)
               .IsRequired(); 

            builder.Property(p => p.SecondName)
              .HasMaxLength(20)
              .IsRequired();

            builder.Property(p => p.ThirdName)
              .HasMaxLength(20)
              .IsRequired(false);

            builder.Property(p => p.LastName)
              .HasMaxLength(20)
              .IsRequired();

            builder.Property(p => p.DateOfBirth)
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(p => p.Gender)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(p => p.Address)
              .HasMaxLength(500)
              .IsRequired();

            builder.Property(p => p.Phone)
              .HasMaxLength(20)
              .IsRequired();

            builder.Property(p => p.Email)
              .HasMaxLength(50)
              .IsRequired(false);

            builder.Property(p => p.NationalityCountryId)
              .IsRequired();

            builder.Property(p => p.ImagePath)
              .HasMaxLength(250)
              .IsRequired(false);

            //Indexes
            builder.HasIndex(p => p.NationalNo).IsUnique();
            builder.HasIndex(p => p.NationalityCountryId);

            //Relationships
            builder.HasOne(p => p.Country)
                .WithMany(c => c.People)
                .HasForeignKey(p => p.NationalityCountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.CreatedByUserId)
                .IsRequired();

            builder.HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.CreatedByUserId);
        }
    }
}

