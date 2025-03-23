using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Enums;

namespace Appointment_System.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public string PatientId { get; set; } // FK to ApplicationUser

        public string DoctorId { get; set; } // FK to ApplicationUser

        public string? GuestEmail { get; set; } // If the appointment is for a guest

        public DateTime AppointmentTime { get; set; }  // Store only Year-Month-Day Hour

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    }

}
