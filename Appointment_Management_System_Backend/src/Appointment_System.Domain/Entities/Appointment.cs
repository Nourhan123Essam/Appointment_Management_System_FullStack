using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Enums;

namespace Appointment_System.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid Id { get; set; }
        public string? PatientId { get; set; }
        public string DoctorId { get; set; } = null!;
        public Guid OfficeId { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }
        public DateTime DateTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public AppointmentType Type { get; set; }
        public Guid? ParentAppointmentId { get; set; }
        public Guid? ChatId { get; set; }
        public Guid? PrescriptionId { get; set; }
        public string? Notes { get; set; }
        public string? PaymentId { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }

        // Navigation
        public virtual Patient? Patient { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;
        public virtual Office Office { get; set; } = null!;
        public virtual Appointment? ParentAppointment { get; set; }
        public virtual ICollection<Appointment> FollowUpAppointments { get; set; } = new List<Appointment>();
        public virtual Chat? Chat { get; set; }
        public virtual Prescription? Prescription { get; set; }
        public virtual Feedback? Feedback { get; set; }
    }

}
