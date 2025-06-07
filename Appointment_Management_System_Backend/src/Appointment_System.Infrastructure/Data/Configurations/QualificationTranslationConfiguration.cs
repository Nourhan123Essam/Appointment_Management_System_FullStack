using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class QualificationTranslationConfiguration : IEntityTypeConfiguration<QualificationTranslation>
    {
        public void Configure(EntityTypeBuilder<QualificationTranslation> builder)
        {
            builder.HasKey(qt => qt.Id);

            builder.Property(d => d.Language)
                    .HasConversion(
                        lang => lang.Value,               // to store in DB
                        value => Language.From(value))    // to read from DB
                    .HasMaxLength(5); // optional, depends on your values

            builder.HasOne(qt => qt.Qualification)
                   .WithMany(q => q.Translations)
                   .HasForeignKey(qt => qt.QualificationId);
        }
    }

}
