using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.DTOs.Specialization;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Enums;

namespace Appointment_System.Application.DTOs.Doctor
{
    public record DoctorsBasicDataDto
    {
        public int Id { get; init; }
        public string UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; }
        public int? YearsOfExperience { get; init; }
        public string? Bio { get; set; }
        public decimal InitialFee { get; set; }
        public decimal FollowUpFee { get; set; }
        public int MaxFollowUps { get; set; }
        public int? RatingPoints { get; set; } = 0;
        public int? NumberOfRatings { get; set; } = 0;
        public int? FollowUpCount { get; set; } = 0;
        public List<SpecializationWithTranslationsDto> DoctorSpecializations { get; set; } = new();

        // Empty constructor (for cases where mapping happens in service)
        public DoctorsBasicDataDto() { }

        // Constructor that maps from ApplicationUser
        public DoctorsBasicDataDto(Domain.Entities.Doctor doctor)
        {
            UserId = doctor.UserId;
            Id = doctor.Id;
            FirstName = doctor.FirstName;
            LastName = doctor.LastName;
            Email = doctor.Email;
            YearsOfExperience = doctor.YearsOfExperience;
            InitialFee = doctor.InitialFee;
            FollowUpFee = doctor.FollowUpFee;
            RatingPoints = doctor.RatingPoints;
            MaxFollowUps = doctor.MaxFollowUps;
            Bio = doctor.Bio;
            NumberOfRatings = doctor.NumberOfRatings;
        }
    }

}
