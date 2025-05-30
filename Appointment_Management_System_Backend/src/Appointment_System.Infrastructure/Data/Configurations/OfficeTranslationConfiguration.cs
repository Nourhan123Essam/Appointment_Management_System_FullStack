using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class OfficeTranslationConfiguration : IEntityTypeConfiguration<OfficeTranslation>
    {
        public void Configure(EntityTypeBuilder<OfficeTranslation> builder)
        {
            builder.HasKey(t => t.Id);

            builder.OwnsOne(t => t.Language, lang =>
            {
                lang.Property(l => l.Value)
                    .HasColumnName("Language") // Column name is 'Language' (string)
                    .HasMaxLength(10)
                    .IsRequired();
            });

            // Optional: prevent EF from creating a nav property named Language again
            builder.Navigation(t => t.Language).IsRequired();
        }
    }
}
