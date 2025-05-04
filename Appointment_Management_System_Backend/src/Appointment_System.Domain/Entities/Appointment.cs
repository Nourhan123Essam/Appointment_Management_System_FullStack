using Appointment_System.Domain.Enums;

namespace Appointment_System.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int DoctorId { get; set; }
        public int OfficeId { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }
        public DateTime DateTime { get; set; } // Exact date and time of appointment
        public AppointmentStatus Status { get; set; }
        public AppointmentType Type { get; set; }
        public int? ParentAppointmentId { get; set; }
        public int? PrescriptionId { get; set; }
        public string? Notes { get; set; }

        // Paypal
        public string? PaymentId { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }

        // Navigation properties
        public Patient? Patient { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public Office Office { get; set; } = null!;
        public Appointment? ParentAppointment { get; set; }
        public ICollection<Appointment> FollowUpAppointments { get; set; } = new List<Appointment>();
        public Chat? Chat { get; set; }
        public Prescription? Prescription { get; set; }
        public Feedback? Feedback { get; set; }
    }
}
