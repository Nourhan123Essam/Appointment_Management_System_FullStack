namespace Appointment_System.Domain.Entities
{
    public class Availability : BaseEntity
    {
        public int Id { get; set; }

        // Foreign Keys
        public int DoctorId { get; set; }
        public int OfficeId { get; set; }

        // Schedule Info
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Navigation Properties
        public Doctor Doctor { get; set; } = null!;
        public Office Office { get; set; } = null!;
        public ICollection<AvailabilityException> AvailabilityExceptions { get; set; } = new List<AvailabilityException>();
    }

}
