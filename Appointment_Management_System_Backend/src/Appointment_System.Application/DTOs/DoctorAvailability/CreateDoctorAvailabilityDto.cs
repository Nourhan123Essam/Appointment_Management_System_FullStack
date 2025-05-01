using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.DTOs.DoctorAvailability
{
    public class CreateDoctorAvailabilityDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DoctorId { get; set; }

        public Domain.Entities.DoctorAvailability ToEntity()
        {
            return new Domain.Entities.DoctorAvailability()
            {
                Id = Guid.Empty,
                DayOfWeek = DayOfWeek,  
                StartTime = StartTime,
                EndTime = EndTime,
                DoctorId = DoctorId
            };
        }
    }
}
