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
    public record DoctorQualificationDto
    {
        public int Id { get; init; }
        public string QualificationName { get; init; }
        public string? IssuingInstitution { get; init; }
        public int YearEarned { get; init; }
        public string DoctorId { get; init; }

        // Parameterless constructor (needed for deserialization)
        public DoctorQualificationDto() { }

        // Constructor for mapping from entity
        public DoctorQualificationDto(Domain.Entities.DoctorQualification entity)
        {
            Id = entity.Id;
            QualificationName = entity.QualificationName;
            IssuingInstitution = entity.IssuingInstitution;
            YearEarned = entity.YearEarned;
            DoctorId = entity.DoctorId;
        }
    }


}
