using Appointment_System.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Appointment_System.Infrastructure.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.ChatId).IsRequired();
            builder.Property(m => m.SenderId).IsRequired();
            builder.Property(m => m.Content).IsRequired().HasMaxLength(2000);

            // SentAt tracks the actual user send time (used in chat UI), while CreatedAt is for DB auditing.
            builder.Property(m => m.SentAt).IsRequired();
            builder.Property(m => m.CreatedAt).IsRequired();

            builder.Property(m => m.IsDeleted).HasDefaultValue(false);

            builder.HasOne(m => m.Chat)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.ChatId);

            // Link SenderId to AspNetUsers table to allow any user type (doctor or patient) to send messages
            builder.HasOne<IdentityUser>()
                   .WithMany()
                   .HasForeignKey(m => m.SenderId);

            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }

}
