using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Office : BaseEntity
    {
        public int Id { get; set; }

        // Navigation properties
        public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<OfficeTranslation> Translations { get; set; } = new List<OfficeTranslation>();
    }

}
