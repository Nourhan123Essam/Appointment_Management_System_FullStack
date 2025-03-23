using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class DoctorAvailability
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }  // Enum for Monday, Tuesday, etc.
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string DoctorId { get; set; } // FK
    }

}
