using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Domain.Enums;
using Appointment_System.Infrastructure.Data;

namespace Appointment_System.Application.DTOs.Doctor
{
    public record DoctorDto
    {
        public string Id { get; init; }
        public string FullName { get; init; }
        public string Email { get; set; }
        public int? YearsOfExperience { get; init; }
        public string? Specialization { get; init; }
        public string? LicenseNumber { get; init; }
        public decimal? ConsultationFee { get; init; }
        public WorkplaceType? WorkplaceType { get; init; }
        public int? TotalRatingScore { get; set; } = 0;
        public int? TotalRatingsGiven { get; set; } = 0;
        public List<DoctorQualificationDto> Qualifications { get; init; } = new();
        public List<DoctorAvailabilityDto> Availabilities { get; init; } = new();

        // Empty constructor (for cases where mapping happens in service)
        public DoctorDto() { }

        // Constructor that maps from ApplicationUser
        public DoctorDto(ApplicationUser doctor)
        {
            Id = doctor.Id;
            FullName = doctor.FullName;
            Email = doctor.Email;                 
            YearsOfExperience = doctor.YearsOfExperience;
            Specialization = doctor.Specialization;
            LicenseNumber = doctor.LicenseNumber;
            ConsultationFee = doctor.ConsultationFee;
            WorkplaceType = doctor.WorkplaceType;
            TotalRatingScore = doctor.TotalRatingScore;
            TotalRatingsGiven = doctor.TotalRatingsGiven;
            Qualifications = doctor.Qualifications.Select(q => new DoctorQualificationDto(q)).ToList();
            Availabilities = doctor.Availabilities.Select(a => new DoctorAvailabilityDto(a)).ToList();
        }
    }


}
