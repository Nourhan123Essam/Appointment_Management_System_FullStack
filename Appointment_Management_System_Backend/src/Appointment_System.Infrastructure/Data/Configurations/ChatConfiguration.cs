using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(c => c.Id);

            // FK to Appointment (1:1 relationship)
            builder.Property(c => c.AppointmentId).IsRequired();
            builder.HasOne(c => c.Appointment)
                    .WithOne(a => a.Chat)
                    .HasForeignKey<Chat>(c => c.AppointmentId)
                    .IsRequired();

            // Denormalized FKs for faster message sender validation
            builder.Property(c => c.DoctorId).IsRequired();
            builder.Property(c => c.PatientId).IsRequired();

            // Ensure one chat per appointment
            builder.HasIndex(c => c.AppointmentId).IsUnique();

            builder.Property(c => c.IsDeleted).HasDefaultValue(false);
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }

}
