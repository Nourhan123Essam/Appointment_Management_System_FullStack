using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Appointment_System.Infrastructure.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        // Only for doctors
        public int? YearsOfExperience { get; set; } 
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public decimal? ConsultationFee { get; set; }
        public WorkplaceType? WorkplaceType { get; set; }  

        public int? TotalRatingScore { get; set; } = 0;
        public int? TotalRatingsGiven { get; set; } = 0;

        // Navigation properties (One-to-Many relationships)
        public ICollection<DoctorQualification> Qualifications { get; set; } = new List<DoctorQualification>();
        public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();


    }

}
