using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class Feedback : BaseEntity
    {
        public Guid Id { get; set; }
        public string DoctorId { get; set; } = null!;
        public Guid AppointmentId { get; set; }
        public int Rate { get; set; }
        public string? Message { get; set; }
        public bool? Recommend { get; set; }

        // Navigation
        public virtual Doctor Doctor { get; set; } = null!;
        public virtual Appointment Appointment { get; set; } = null!;
    }
}
