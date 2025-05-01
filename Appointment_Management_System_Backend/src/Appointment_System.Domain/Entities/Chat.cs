using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class Chat : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }

        // Navigation
        public virtual Appointment Appointment { get; set; } = null!;
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
