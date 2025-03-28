using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Domain.Enums;

namespace Appointment_System.Application.DTOs.Doctor
{
    public record DoctorDto(
         string Id,
         string FullName,
         int? YearsOfExperience,
         string? Specialization,
         string? LicenseNumber,
         decimal? ConsultationFee,
         WorkplaceType? WorkplaceType,
         List<DoctorQualificationDto> Qualifications,
         List<DoctorAvailabilityDto> Availabilities
     );

}
