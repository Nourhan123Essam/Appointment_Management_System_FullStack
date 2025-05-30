using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class SpecializationTranslationConfiguration : IEntityTypeConfiguration<SpecializationTranslation>
    {
        public void Configure(EntityTypeBuilder<SpecializationTranslation> builder)
        {
            builder.HasKey(t => t.Id);

            builder.OwnsOne(t => t.LanguageValue, lang =>
            {
                lang.Property(l => l.Value)
                    .HasColumnName("Language") // Keep the column name same
                    .HasMaxLength(10)
                    .IsRequired();
            });

            builder
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);
        }
    }

}
