using Appointment_System.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.InitialFee).IsRequired().HasPrecision(18, 2);
            builder.Property(d => d.FollowUpFee).IsRequired().HasPrecision(18, 2);
            builder.Property(d => d.MaxFollowUps).IsRequired();
            builder.Property(d => d.RatingPoints).HasDefaultValue(0);
            builder.Property(d => d.NumberOfRatings).HasDefaultValue(0);
            builder.Property(d => d.IsDeleted).HasDefaultValue(false);
            builder.Property(d => d.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(d => d.UserId).IsUnique();

            builder.HasQueryFilter(d => !d.IsDeleted);

            builder.HasOne<IdentityUser>()
                   .WithOne()
                   .HasForeignKey<Doctor>(d => d.UserId)
                   .IsRequired();

            builder.HasMany(d => d.Qualifications)
                   .WithOne(q => q.Doctor)
                   .HasForeignKey(q => q.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.Feedbacks)
                   .WithOne(f => f.Doctor)
                   .HasForeignKey(f => f.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.Appointments)
                   .WithOne(a => a.Doctor)
                   .HasForeignKey(a => a.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.Availabilities)
                   .WithOne(a => a.Doctor)
                   .HasForeignKey(a => a.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Add new relationship for Doctor Translations
            builder.HasMany(d => d.Translations)
                   .WithOne(t => t.Doctor)
                   .HasForeignKey(t => t.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
