using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Medicine : BaseEntity
    {
        public int Id { get; set; }

        // Foreign key to the parent prescription
        public int PrescriptionId { get; set; }

        public string Name { get; set; } = null!;
        public string Dosage { get; set; } = null!;

        // Optional usage instructions
        public string? Instructions { get; set; }

        // Navigation property to parent prescription
        public Prescription Prescription { get; set; } = null!;
    }

}
