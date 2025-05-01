using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class Specialization : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        // Navigation
        public virtual ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
    }
}
