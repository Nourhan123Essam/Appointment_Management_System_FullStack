
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.DTOs.Patient
{
    public record PatientDto
    {
        public int Id { get; set; } = 0;
        public string UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int AppointmentCount { get; set; } = 0;

        public PatientDto()
        {
            
        }
        public PatientDto(Domain.Entities.Patient patient)
        {
            Id = patient.Id;
            UserId = patient.UserId;
            FullName = patient.FirstName + patient.LastName;
            Email = patient.Email;
            Gender = patient.Gender;
            DateOfBirth = patient.DateOfBirth;
            Address = patient.Address;
            CreatedAt = patient.CreatedAt;
        }
       
    }

}
