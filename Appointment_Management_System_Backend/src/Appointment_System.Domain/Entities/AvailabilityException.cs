namespace Appointment_System.Domain.Entities
{
    public class AvailabilityException : BaseEntity
    {
        public int Id { get; set; }
        public int AvailabilityId { get; set; }
        public DateOnly Date { get; set; }

        // Optional reason for the exception (e.g., doctor on leave, emergency, etc.)
        public string? Reason { get; set; }

        // Navigation
        public Availability Availability { get; set; } = null!;
    }

}
