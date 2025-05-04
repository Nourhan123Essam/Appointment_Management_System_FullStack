using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class AvailabilityExceptionConfiguration : IEntityTypeConfiguration<AvailabilityException>
    {
        public void Configure(EntityTypeBuilder<AvailabilityException> builder)
        {
            builder.HasKey(ae => ae.Id);

            builder.Property(ae => ae.AvailabilityId).IsRequired();
            builder.Property(ae => ae.Date).IsRequired();
            builder.Property(ae => ae.Reason).HasMaxLength(500); // Optional

            builder.HasOne(ae => ae.Availability)
                   .WithMany(a => a.AvailabilityExceptions)
                   .HasForeignKey(ae => ae.AvailabilityId);

            builder.Property(ae => ae.IsDeleted).HasDefaultValue(false);
            builder.Property(ae => ae.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            builder.HasQueryFilter(ae => !ae.IsDeleted);
        }
    }

}
