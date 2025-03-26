using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.DTOs.DoctorQualification
{
    // DTO for adding a new qualification. Uses a class for flexibility in modification.
    public class CreateDoctorQualificationDto
    {
        public string QualificationName { get; set; }
        public string IssuingInstitution { get; set; }
        public int YearEarned { get; set; }
        public string DoctorId { get; set; }

        // Maps this DTO to a new DoctorQualification entity.
        // Keeps mapping logic separate for cleaner service code.
        public Domain.Entities.DoctorQualification ToEntity()
        {
            return new Domain.Entities.DoctorQualification
            {
                QualificationName = QualificationName,
                IssuingInstitution = IssuingInstitution,
                YearEarned = YearEarned,
                DoctorId = DoctorId
            };
        }
    }

}
