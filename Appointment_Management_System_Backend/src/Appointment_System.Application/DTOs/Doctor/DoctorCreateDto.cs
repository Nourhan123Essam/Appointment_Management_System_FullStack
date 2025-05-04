using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Enums;

namespace Appointment_System.Application.DTOs.Doctor
{
    public class DoctorCreateDto
    {
        public string Password { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public decimal InitialFee { get; set; }
        public decimal FollowUpFee { get; set; }
        public int MaxFollowUps { get; set; }
        public int YearsOfExperience { get; set; }
        public int? FollowUpCount { get; set; } = 0;

        public List<DoctorAvailabilityDto> Availabilities { get; set; } = new();
        public List<DoctorQualificationDto> Qualifications { get; set; } = new();
        public virtual List<DoctorSpecialization> DoctorSpecializations { get; set; } = new();
    }

}
