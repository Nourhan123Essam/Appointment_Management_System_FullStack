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
            builder.Property(o => o.IsDeleted).HasDefaultValue(false);
            builder.Property(o => o.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasMany(o => o.Availabilities)
                   .WithOne(a => a.Office)
                   .HasForeignKey(a => a.OfficeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Appointments)
                   .WithOne(a => a.Office)
                   .HasForeignKey(a => a.OfficeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Translations)
                   .WithOne(t => t.Office)
                   .HasForeignKey(t => t.OfficeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Soft Delete Filter
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
