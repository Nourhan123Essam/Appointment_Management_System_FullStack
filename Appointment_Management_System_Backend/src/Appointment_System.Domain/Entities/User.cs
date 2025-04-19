
using Appointment_System.Domain.Enums;

namespace Appointment_System.Domain.Entities
{
    public class User
    {
        public string Id { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = default!;

        // Doctor-specific
        public int? YearsOfExperience { get; set; }
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public decimal? ConsultationFee { get; set; }
        public WorkplaceType? WorkplaceType { get; set; }
        public int? TotalRatingScore { get; set; } = 0;
        public int? TotalRatingsGiven { get; set; } = 0;

        // Optional for all roles
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
    }
}
