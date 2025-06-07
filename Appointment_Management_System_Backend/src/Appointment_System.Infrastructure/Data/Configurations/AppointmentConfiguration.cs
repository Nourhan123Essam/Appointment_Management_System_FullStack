using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(a => a.Id);

            // Required fields
            builder.Property(a => a.DoctorId).IsRequired();
            builder.Property(a => a.OfficeId).IsRequired();
            builder.Property(a => a.DateTime).IsRequired();
            builder.Property(a => a.Status).IsRequired();
            builder.Property(a => a.Type).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();

            // Optional fields with constraints
            builder.Property(a => a.GuestName).HasMaxLength(200);
            builder.Property(a => a.GuestEmail).HasMaxLength(100);
            builder.Property(a => a.GuestPhone).HasMaxLength(20);
            builder.Property(a => a.Notes).HasMaxLength(1000);
            builder.Property(a => a.PaymentId).HasMaxLength(100);

            // Relationships
            builder.HasOne(a => a.Patient)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey(a => a.PatientId)
                   .IsRequired(false);

            builder.HasOne(a => a.Doctor)
                   .WithMany(d => d.Appointments)
                   .HasForeignKey(a => a.DoctorId);

            builder.HasOne(a => a.Office)
                   .WithMany(o => o.Appointments)
                   .HasForeignKey(a => a.OfficeId);

            builder.HasOne(a => a.ParentAppointment)
                   .WithMany(p => p.FollowUpAppointments)
                   .HasForeignKey(a => a.ParentAppointmentId)
                   .IsRequired(false);

            builder.HasOne(a => a.Prescription)
                   .WithOne(p => p.Appointment)
                   .HasForeignKey<Appointment>(a => a.PrescriptionId)
                   .IsRequired(false);

            // Ensure no duplicate appointment time per doctor
            builder.HasIndex(a => new { a.DoctorId, a.DateTime }).IsUnique();

            // Soft delete support
            builder.Property(a => a.IsDeleted).HasDefaultValue(false);
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }

}
