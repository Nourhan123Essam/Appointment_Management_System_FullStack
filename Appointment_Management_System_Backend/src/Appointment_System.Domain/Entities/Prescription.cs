using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Prescription : BaseEntity
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }

        // Navigation properties
        public Appointment Appointment { get; set; } = null!;
        public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
    }
}
