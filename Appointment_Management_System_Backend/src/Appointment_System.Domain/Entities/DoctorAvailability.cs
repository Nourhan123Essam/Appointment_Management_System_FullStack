using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class DoctorAvailability: BaseEntity
    {
        public Guid Id { get; set; }
        public string DoctorId { get; set; } = null!;
        public Guid OfficeId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Navigation
        public virtual Doctor Doctor { get; set; } = null!;
        public virtual Office Office { get; set; } = null!;
        public virtual ICollection<AvailabilityException> AvailabilityExceptions { get; set; } = new List<AvailabilityException>();
    }

}
