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

            builder.Property(s => s.IsDeleted)
                   .HasDefaultValue(false); // Soft delete flag

            builder.Property(s => s.CreatedAt)
                   .IsRequired() // Ensures CreatedAt is required
                   .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time on creation

            // Soft Delete: Ensure only non-deleted specializations are retrieved in queries
            builder.HasQueryFilter(s => !s.IsDeleted);

            // Relationship with DoctorSpecialization 
            builder.HasMany(s => s.DoctorSpecializations)
                   .WithOne(ds => ds.Specialization)
                   .HasForeignKey(ds => ds.SpecializationId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of specialization if it’s assigned to doctors

            builder
                .HasMany(s => s.Translations)
                .WithOne(t => t.Specialization)
                .HasForeignKey(t => t.SpecializationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }


}
