using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class DoctorTranslationConfiguration : IEntityTypeConfiguration<DoctorTranslation>
    {
        public void Configure(EntityTypeBuilder<DoctorTranslation> builder)
        {
            builder.HasKey(dt => dt.Id);

            builder.Property(d => d.Language)
                    .HasConversion(
                        lang => lang.Value,               // to store in DB
                        value => Language.From(value))    // to read from DB
                    .HasMaxLength(5); // optional, depends on your values

            builder.HasOne(dt => dt.Doctor)
                   .WithMany(d => d.Translations)
                   .HasForeignKey(dt => dt.DoctorId);
        }
    }

}
