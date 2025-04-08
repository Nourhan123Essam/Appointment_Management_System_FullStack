using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class DoctorQualification
    {
        public int Id { get; set; }
        public string QualificationName { get; set; }
        public string IssuingInstitution { get; set; }  // (Optional) Example: Harvard Medical School
        public int YearEarned { get; set; } // Example: 2015

        public string DoctorId { get; set; } // FK
    }

}
