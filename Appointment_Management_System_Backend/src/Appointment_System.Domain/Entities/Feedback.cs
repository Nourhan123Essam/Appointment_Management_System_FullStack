using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Feedback : BaseEntity
    {
        public int Id { get; set; }
        public int Rate { get; set; }
        public string? Message { get; set; }
        public bool? Recommend { get; set; }

        public int DoctorId { get; set; }

        // Navigation
        public virtual Doctor Doctor { get; set; } = new();
    }
}
