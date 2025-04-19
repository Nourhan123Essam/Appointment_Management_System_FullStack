
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Appointment_System.Infrastructure.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        // Doctor-specific
        public int? YearsOfExperience { get; set; }
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public decimal? ConsultationFee { get; set; }
        public WorkplaceType? WorkplaceType { get; set; }
        public int? TotalRatingScore { get; set; } = 0;
        public int? TotalRatingsGiven { get; set; } = 0;

        // Common optional properties
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Navigation Properties
        public ICollection<DoctorQualification> Qualifications { get; set; } = new List<DoctorQualification>();
        public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();
        public ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
        public ICollection<Appointment> DoctorAppointments { get; set; } = new List<Appointment>();

        // Empty constructor (required by EF Core and Identity)
        public ApplicationUser() { }

        // Constructor that maps from Domain.User
        public ApplicationUser(User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            FullName = user.FullName;

            YearsOfExperience = user.YearsOfExperience;
            Specialization = user.Specialization;
            LicenseNumber = user.LicenseNumber;
            ConsultationFee = user.ConsultationFee;
            WorkplaceType = user.WorkplaceType;
            TotalRatingScore = user.TotalRatingScore;
            TotalRatingsGiven = user.TotalRatingsGiven;

            CreatedAt = user.CreatedAt;
            Gender = user.Gender;
            DateOfBirth = user.DateOfBirth;
            Address = user.Address;
            ProfilePictureUrl = user.ProfilePictureUrl;

            // Navigation properties are NOT mapped here by default.
            // You can handle them explicitly if needed later.
        }

        // Convert ApplicationUser to Domain.User
        public User ToDomain()
        {
            return new User
            {
                Id = this.Id,
                UserName = this.UserName,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                FullName = this.FullName,

                YearsOfExperience = this.YearsOfExperience,
                Specialization = this.Specialization,
                LicenseNumber = this.LicenseNumber,
                ConsultationFee = this.ConsultationFee,
                WorkplaceType = this.WorkplaceType,
                TotalRatingScore = this.TotalRatingScore,
                TotalRatingsGiven = this.TotalRatingsGiven,

                CreatedAt = this.CreatedAt,
                Gender = this.Gender,
                DateOfBirth = this.DateOfBirth,
                Address = this.Address,
                ProfilePictureUrl = this.ProfilePictureUrl,

                // Navigation properties can be added here if needed
                Qualifications = this.Qualifications?.ToList() ?? new List<DoctorQualification>(),
                Availabilities = this.Availabilities?.ToList() ?? new List<DoctorAvailability>(),
                PatientAppointments = this.PatientAppointments?.ToList() ?? new List<Appointment>(),
                DoctorAppointments = this.DoctorAppointments?.ToList() ?? new List<Appointment>()
            };
        }


    }

}
