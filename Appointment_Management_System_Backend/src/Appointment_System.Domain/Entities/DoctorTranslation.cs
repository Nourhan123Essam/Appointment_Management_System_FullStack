using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Domain.Entities
{
    public class DoctorTranslation
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public Language Language { get; set; } = null!;

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Bio { get; set; } = null!;

        public Doctor Doctor { get; set; } = new();
    }

}
