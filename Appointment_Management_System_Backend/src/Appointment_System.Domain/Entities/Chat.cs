using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Chat : BaseEntity
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }

        // Duplicated from Appointment for fast sender validation and to avoid deep joins during messaging
        public int DoctorId { get; set; }  // For fast Sender validation
        public int PatientId { get; set; } // For fast Sender validation

        public Appointment Appointment { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }

}
