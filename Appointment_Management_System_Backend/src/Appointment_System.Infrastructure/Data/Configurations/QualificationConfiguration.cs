using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class QualificationConfiguration : IEntityTypeConfiguration<Qualification>
    {
        public void Configure(EntityTypeBuilder<Qualification> builder)
        {
            builder.HasKey(q => q.Id);

            builder.Property(q => q.YearEarned).IsRequired();
            builder.Property(q => q.IsDeleted).HasDefaultValue(false);
            builder.Property(q => q.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(q => q.Doctor)
                   .WithMany(d => d.Qualifications)
                   .HasForeignKey(q => q.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Add translations relationship
            builder.HasMany(q => q.Translations)
                   .WithOne(t => t.Qualification)
                   .HasForeignKey(t => t.QualificationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(q => !q.IsDeleted);
        }
    }

}
