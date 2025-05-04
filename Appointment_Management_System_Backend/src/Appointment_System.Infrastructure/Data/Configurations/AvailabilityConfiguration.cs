using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
    {
        public void Configure(EntityTypeBuilder<Availability> builder)
        {
            // Primary Key
            builder.HasKey(a => a.Id);

            // Required fields
            builder.Property(a => a.DoctorId).IsRequired();
            builder.Property(a => a.OfficeId).IsRequired();
            builder.Property(a => a.DayOfWeek).IsRequired();
            builder.Property(a => a.StartTime).IsRequired();
            builder.Property(a => a.EndTime).IsRequired();

            // Relationships
            builder.HasOne(a => a.Doctor)
                   .WithMany(d => d.Availabilities)
                   .HasForeignKey(a => a.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Office)
                   .WithMany(o => o.Availabilities)
                   .HasForeignKey(a => a.OfficeId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(a => a.AvailabilityExceptions)
                   .WithOne(e => e.Availability)
                   .HasForeignKey(e => e.AvailabilityId)
                   .OnDelete(DeleteBehavior.Restrict); 

            // Metadata
            builder.Property(a => a.IsDeleted).HasDefaultValue(false);
            builder.Property(a => a.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            // Query filter for soft delete
            builder.HasQueryFilter(a => !a.IsDeleted);

            // Index for efficient querying
            builder.HasIndex(a => new { a.DoctorId, a.OfficeId, a.DayOfWeek });
        }
    }

}
