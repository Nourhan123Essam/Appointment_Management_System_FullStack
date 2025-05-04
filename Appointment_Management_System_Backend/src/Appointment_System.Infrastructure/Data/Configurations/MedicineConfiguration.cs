using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
    {
        public void Configure(EntityTypeBuilder<Medicine> builder)
        {
            // Primary key
            builder.HasKey(m => m.Id);

            // Foreign key is required
            builder.Property(m => m.PrescriptionId).IsRequired();

            // Required fields with max length
            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.Dosage)
                   .IsRequired()
                   .HasMaxLength(100);

            // Optional field with max length
            builder.Property(m => m.Instructions)
                   .HasMaxLength(500);

            // Many medicines belong to one prescription
            builder.HasOne(m => m.Prescription)
                   .WithMany(p => p.Medicines)
                   .HasForeignKey(m => m.PrescriptionId);

            // Soft delete and audit config
            builder.Property(m => m.IsDeleted).HasDefaultValue(false);
            builder.Property(m => m.CreatedAt).IsRequired();
            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }

}
