using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Application.DTOs.DoctorQualification
{
    public class UpdateDoctorQualificationDto
    {
        public string QualificationName { get; set; }
        public string IssuingInstitution { get; set; }
        public int YearEarned { get; set; }

        // Updates an existing DoctorQualification entity.
        // Prevents direct mapping in the service layer for better separation of concerns.
        public void UpdateEntity(Domain.Entities.Qualification qualification)
        {
            qualification.QualificationName = QualificationName;
            qualification.IssuingInstitution = IssuingInstitution;
            qualification.YearEarned = YearEarned;
        }
    }

}
