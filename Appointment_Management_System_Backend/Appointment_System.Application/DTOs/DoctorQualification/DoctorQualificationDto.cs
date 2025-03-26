using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Application.DTOs.DoctorQualification
{
    /// <summary>
    /// DTO for fetching doctor qualifications (immutable for safety and efficiency).
    /// Uses a record for value-based equality and cleaner mapping.
    /// </summary>
    public record DoctorQualificationDto(int Id, string QualificationName, string? IssuingInstitution, int YearEarned, string DoctorId)
    {
        public DoctorQualificationDto(Domain.Entities.DoctorQualification entity)
            : this(entity.Id, entity.QualificationName, entity.IssuingInstitution, entity.YearEarned, entity.DoctorId) { }
    }

}
