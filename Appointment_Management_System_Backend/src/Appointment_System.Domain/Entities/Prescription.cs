using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class Prescription : BaseEntity
    {
        public Guid AppointmentId { get; set; }

        // Navigation
        public virtual Appointment Appointment { get; set; } = null!;
        public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
    }
}
