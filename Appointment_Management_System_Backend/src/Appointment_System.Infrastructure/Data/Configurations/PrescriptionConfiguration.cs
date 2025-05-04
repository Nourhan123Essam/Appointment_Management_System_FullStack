using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            // Use AppointmentId as both PK and FK (1-to-1 relationship)
            builder.HasKey(p => p.AppointmentId);

            // Required one-to-one: Appointment must exist for each Prescription
            builder.HasOne(p => p.Appointment)
                   .WithOne(a => a.Prescription)
                   .HasForeignKey<Prescription>(p => p.AppointmentId);

            // Soft delete default
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            // Ensure creation timestamp
            builder.Property(p => p.CreatedAt).IsRequired();

            // Global query filter to exclude soft-deleted records
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }

}
