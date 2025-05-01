using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Application.DTOs.Appointment
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string DoctorName { get; set; } // Optional: include more info if needed
    }
}
