using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class AvailabilityException : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid AvailabilityId { get; set; }
        public DateOnly Date { get; set; }

        // Navigation
        public virtual DoctorAvailability Availability { get; set; } = null!;
    }
}
