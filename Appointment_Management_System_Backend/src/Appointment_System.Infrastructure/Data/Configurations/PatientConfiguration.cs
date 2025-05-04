using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Phone).IsRequired().HasMaxLength(20);
            builder.Property(p => p.DateOfBirth).IsRequired();
            builder.Property(p => p.Address).HasMaxLength(500);
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);
            builder.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            // Index on UserId
            builder.HasIndex(p => p.UserId).IsUnique();

            // One-to-One with AspNetUsers
            builder.HasOne<IdentityUser>()
                   .WithOne()
                   .HasForeignKey<Patient>(p => p.UserId)
                   .IsRequired();

            // One-to-Many with Appointment
            builder.HasMany(p => p.Appointments)
                   .WithOne(a => a.Patient)
                   .HasForeignKey(a => a.PatientId);

            // Global query filter for soft delete
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
