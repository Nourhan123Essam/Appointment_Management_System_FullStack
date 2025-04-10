
using Appointment_System.Infrastructure.Data;

namespace Appointment_System.Application.DTOs.Patient
{
    public record PatientDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AppointmentCount { get; set; } = -1;

        public PatientDto()
        {
            
        }
        public PatientDto(ApplicationUser patient)
        {
            Id = patient.Id;
            FullName = patient.FullName;
            Email = patient.Email;
            Gender = patient.Gender;
            DateOfBirth = patient.DateOfBirth;
            Address = patient.Address;
            ProfilePictureUrl = patient.ProfilePictureUrl;
            CreatedAt = patient.CreatedAt;
        }
       
    }

}
