using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.HasKey(f => f.Id);

            // Denormalized DoctorId to improve query performance when fetching doctor feedbacks.
            builder.Property(f => f.DoctorId).IsRequired();

            builder.Property(f => f.AppointmentId).IsRequired();
            builder.Property(f => f.Rate).IsRequired();
            builder.Property(f => f.Message).HasMaxLength(1000);
            builder.Property(f => f.IsDeleted).HasDefaultValue(false);
            builder.Property(f => f.CreatedAt).IsRequired();

            builder.HasOne(f => f.Doctor)
                   .WithMany(d => d.Feedbacks)
                   .HasForeignKey(f => f.DoctorId);

            builder.HasOne(f => f.Appointment)
                   .WithOne(a => a.Feedback)
                   .HasForeignKey<Feedback>(f => f.AppointmentId);

            builder.HasIndex(f => f.AppointmentId).IsUnique();
            builder.HasQueryFilter(f => !f.IsDeleted);
        }
    }

}
