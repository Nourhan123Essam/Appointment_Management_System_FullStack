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
            // In the configuration file:
            builder.HasKey(d => d.Id); // Primary Key for Doctor


            // Properties
            builder.Property(d => d.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.LastName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Bio).HasMaxLength(1000);
            builder.Property(d => d.InitialFee).IsRequired().HasPrecision(18, 2);
            builder.Property(d => d.FollowUpFee).IsRequired().HasPrecision(18, 2);
            builder.Property(d => d.MaxFollowUps).IsRequired();
            builder.Property(d => d.RatingPoints).HasDefaultValue(0);
            builder.Property(d => d.NumberOfRatings).HasDefaultValue(0);
            builder.Property(d => d.IsDeleted).HasDefaultValue(false);
            builder.Property(d => d.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            // Index for faster lookups
            builder.HasIndex(d => d.UserId).IsUnique();


            // Foreign key relationship to IdentityUser (AspNetUsers)
            builder.HasOne<IdentityUser>()
                   .WithOne()
                   .HasForeignKey<Doctor>(d => d.UserId)
                   .IsRequired(); // Ensuring that UserId is required

            // Relationships
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

            // Soft delete filter
            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }

}
