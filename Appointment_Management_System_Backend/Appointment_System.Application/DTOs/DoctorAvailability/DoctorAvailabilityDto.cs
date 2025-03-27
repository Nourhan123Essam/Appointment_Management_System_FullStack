using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.DTOs.DoctorAvailability
{
    public class DoctorAvailabilityDto
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DoctorId { get; set; }

        public DoctorAvailabilityDto()
        {
            
        }
        // Constructor that takes the entity as input
        public DoctorAvailabilityDto(Domain.Entities.DoctorAvailability availability)
        {
            Id = availability.Id;
            DayOfWeek = availability.DayOfWeek;
            StartTime = availability.StartTime;
            EndTime = availability.EndTime;
            DoctorId = availability.DoctorId;
        }
    }
}
