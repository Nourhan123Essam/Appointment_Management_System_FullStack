using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class DoctorSpecialization : BaseEntity
    {
        public int DoctorId { get; set; }
        public int SpecializationId { get; set; }

        // Navigation
        public Doctor Doctor { get; set; } = null!;
        public Specialization Specialization { get; set; } = null!;
    }
}
