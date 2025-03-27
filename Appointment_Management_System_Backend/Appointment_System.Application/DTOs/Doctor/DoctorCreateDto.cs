using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Domain.Enums;

namespace Appointment_System.Application.DTOs.Doctor
{
    public class DoctorCreateDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }  // Will be hashed in the service layer
        public int YearsOfExperience { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public decimal ConsultationFee { get; set; }
        public WorkplaceType WorkplaceType { get; set; }

        public List<DoctorAvailabilityDto> Availability { get; set; } = new();
        public List<DoctorQualificationDto> Qualifications { get; set; } = new();
    }

}
