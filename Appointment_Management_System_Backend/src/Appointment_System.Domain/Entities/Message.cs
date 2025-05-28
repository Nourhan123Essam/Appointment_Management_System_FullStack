using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Message : BaseEntity
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public string SenderId { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }

        // Navigation
        public virtual Chat Chat { get; set; } = null!;
    }
}
