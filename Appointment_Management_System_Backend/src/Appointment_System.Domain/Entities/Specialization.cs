using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Specialization : BaseEntity
    {
        public int Id { get; set; }

        // Navigation properties
        public ICollection<SpecializationTranslation> Translations { get; set; } = new List<SpecializationTranslation>();
        public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
    }
}
