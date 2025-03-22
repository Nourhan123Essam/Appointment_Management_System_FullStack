using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Appointment_System.Infrastructure.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public int? YearsOfExperience { get; set; } // Only relevant for doctors
        public string? Specialization { get; set; }  // Only relevant for doctors
        public string? LicenseNumber { get; set; }   // Only relevant for doctors
        public decimal? ConsultationFee { get; set; } // Only relevant for doctors

        // Relationships
        public ICollection<Appointment> BookedAppointments { get; set; } = new List<Appointment>();  // If patient
        public ICollection<Appointment> CheckAppointments { get; set; } = new List<Appointment>();  // If doctor
    }
}
