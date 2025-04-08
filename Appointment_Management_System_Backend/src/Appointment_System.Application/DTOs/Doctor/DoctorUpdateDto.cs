using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Enums;

namespace Appointment_System.Application.DTOs.Doctor
{
    public class DoctorUpdateDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public decimal? ConsultationFee { get; set; }
        public WorkplaceType? WorkplaceType { get; set; }
        public int? TotalRatingScore { get; set; } = 0;
        public int? TotalRatingsGiven { get; set; } = 0;
    }

}
