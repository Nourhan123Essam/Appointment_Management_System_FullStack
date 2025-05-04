using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class QualificationConfiguration : IEntityTypeConfiguration<Qualification>
    {
        public void Configure(EntityTypeBuilder<Qualification> builder)
        {
            // Primary Key
            builder.HasKey(q => q.Id);

            // Properties
            builder.Property(q => q.QualificationName).IsRequired().HasMaxLength(200); // Adjusted from 'Title'
            builder.Property(q => q.IssuingInstitution).IsRequired().HasMaxLength(500); // Adjusted from 'Institution'
            builder.Property(q => q.YearEarned).IsRequired(); // Keep required for the year
            builder.Property(q => q.IsDeleted).HasDefaultValue(false); // Soft delete handling
            builder.Property(q => q.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()"); ; // CreatedAt will be required

            // Relationship: Qualification to Doctor (DoctorId as Foreign Key)
            builder.HasOne(q => q.Doctor)
                   .WithMany(d => d.Qualifications)
                   .HasForeignKey(q => q.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict); // Restricted deletion, as qualifications should not be removed if a doctor is deleted

            // Soft Delete Query Filter (Ensures deleted qualifications are excluded in queries)
            builder.HasQueryFilter(q => !q.IsDeleted);
        }
    }

}
