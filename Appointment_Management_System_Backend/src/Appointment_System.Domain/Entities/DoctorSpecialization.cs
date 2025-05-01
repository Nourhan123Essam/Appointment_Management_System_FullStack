using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class DoctorSpecialization : BaseEntity
    {
        public string DoctorId { get; set; } = null!;
        public Guid SpecializationId { get; set; }

        // Navigation
        public virtual Doctor Doctor { get; set; } = null!;
        public virtual Specialization Specialization { get; set; } = null!;
    }
}
