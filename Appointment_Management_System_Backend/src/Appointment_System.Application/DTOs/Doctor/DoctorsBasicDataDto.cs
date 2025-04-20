using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Enums;

namespace Appointment_System.Application.DTOs.Doctor
{
    public record DoctorsBasicDataDto(
        string Id,
        string FullName,
        string Email,
        int? YearsOfExperience,
        string? Specialization,
        string? LicenseNumber,
        decimal? ConsultationFee,
        WorkplaceType? WorkplaceType,
        int? TotalRatingScore,
        int? TotalRatingsGiven
    )
    {
        public DoctorsBasicDataDto(User doctor) : this(
            doctor.Id,
            doctor.FullName,
            doctor.Email,
            doctor.YearsOfExperience,
            doctor.Specialization,
            doctor.LicenseNumber,
            doctor.ConsultationFee,
            doctor.WorkplaceType,
            doctor.TotalRatingScore,
            doctor.TotalRatingsGiven
        )
        {
        }
    }

}
