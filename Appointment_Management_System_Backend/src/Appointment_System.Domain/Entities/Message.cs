using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class Message : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public string SenderId { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }

        // Navigation
        public virtual Chat Chat { get; set; } = null!;
    }
}
