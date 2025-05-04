using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            // Primary Key
            builder.HasKey(s => s.Id);

            // Properties
            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(100); // Name of specialization (e.g., Cardiologist)

            builder.Property(s => s.IsDeleted)
                   .HasDefaultValue(false); // Soft delete flag

            builder.Property(s => s.CreatedAt)
                   .IsRequired() // Ensures CreatedAt is required
                   .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time on creation

            // Soft Delete: Ensure only non-deleted specializations are retrieved in queries
            builder.HasQueryFilter(s => !s.IsDeleted);

            // Index on Name for faster lookup
            builder.HasIndex(s => s.Name).IsUnique(); // Ensures specialization names are unique

            // Relationship with DoctorSpecialization 
            builder.HasMany(s => s.DoctorSpecializations)
                   .WithOne(ds => ds.Specialization)
                   .HasForeignKey(ds => ds.SpecializationId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of specialization if it’s assigned to doctors
        }
    }


}
