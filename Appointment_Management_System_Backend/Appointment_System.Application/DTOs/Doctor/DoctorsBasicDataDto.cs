using Appointment_System.Domain.Enums;

namespace Appointment_System.Application.DTOs.Doctor
{
    public record DoctorsBasicDataDto(
         string Id,
         string FullName,
         int? YearsOfExperience,
         string? Specialization,
         string? LicenseNumber,
         decimal? ConsultationFee,
         WorkplaceType? WorkplaceType,
         int? TotalRatingScore,
         int? TotalRatingsGiven
     );
}
