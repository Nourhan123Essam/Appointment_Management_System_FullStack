using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class Medicine : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid PrescriptionId { get; set; }
        public string Name { get; set; } = null!;
        public string Dosage { get; set; } = null!;
        public string? Instructions { get; set; }

        // Navigation
        public virtual Prescription Prescription { get; set; } = null!;
    }
}
