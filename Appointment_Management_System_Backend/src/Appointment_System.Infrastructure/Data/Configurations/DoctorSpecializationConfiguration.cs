using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class DoctorSpecializationConfiguration : IEntityTypeConfiguration<DoctorSpecialization>
    {
        public void Configure(EntityTypeBuilder<DoctorSpecialization> builder)
        {
            // Composite Key: DoctorId and SpecializationId form the primary key for the join table
            builder.HasKey(ds => new { ds.DoctorId, ds.SpecializationId });

            // Relationships
            builder.HasOne(ds => ds.Doctor)
                   .WithMany(d => d.DoctorSpecializations)
                   .HasForeignKey(ds => ds.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ds => ds.Specialization)
                   .WithMany(s => s.DoctorSpecializations)
                   .HasForeignKey(ds => ds.SpecializationId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Soft delete and auditing properties
            builder.Property(ds => ds.IsDeleted).HasDefaultValue(false);
            builder.Property(ds => ds.CreatedAt).IsRequired();
            builder.HasQueryFilter(ds => !ds.IsDeleted);

            // Adding Indexes for better performance
            builder.HasIndex(ds => new { ds.DoctorId, ds.SpecializationId }).IsUnique();
        }
    }


}
