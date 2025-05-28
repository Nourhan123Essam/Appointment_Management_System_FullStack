using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Feedback : BaseEntity
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int Rate { get; set; }
        public string? Message { get; set; }
        public bool? Recommend { get; set; }

        // Denormalized DoctorId to improve query performance when fetching doctor feedbacks.
        public int DoctorId { get; set; }

        // Navigation
        public virtual Doctor Doctor { get; set; } = null!;
        public virtual Appointment Appointment { get; set; } = null!;
    }
}
