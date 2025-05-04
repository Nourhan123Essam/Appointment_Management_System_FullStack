using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class OfficeConfiguration : IEntityTypeConfiguration<Office>
    {
        public void Configure(EntityTypeBuilder<Office> builder)
        {
            // Primary Key
            builder.HasKey(o => o.Id);

            // Properties
            builder.Property(o => o.StreetAddress).IsRequired().HasMaxLength(200);
            builder.Property(o => o.City).IsRequired().HasMaxLength(100);
            builder.Property(o => o.State).IsRequired().HasMaxLength(100);
            builder.Property(o => o.Country).IsRequired().HasMaxLength(100);
            builder.Property(o => o.Zip).IsRequired().HasMaxLength(20);
            builder.Property(o => o.IsDeleted).HasDefaultValue(false); // Soft delete flag
            builder.Property(o => o.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasMany(o => o.Availabilities)
                   .WithOne(a => a.Office)
                   .HasForeignKey(a => a.OfficeId)
                   .OnDelete(DeleteBehavior.Restrict); // If Office is deleted, its related Availabilities are deleted

            builder.HasMany(o => o.Appointments)
                   .WithOne(a => a.Office)
                   .HasForeignKey(a => a.OfficeId)
                   .OnDelete(DeleteBehavior.Restrict); // If Office is deleted, its related Appointments are deleted

            // Soft Delete Filter
            builder.HasQueryFilter(o => !o.IsDeleted); // Soft delete filter

            // Add an index on the combination of StreetAddress and City for better performance on search queries
            builder.HasIndex(o => new { o.StreetAddress, o.City }).IsUnique(false);
        }
    }

}
