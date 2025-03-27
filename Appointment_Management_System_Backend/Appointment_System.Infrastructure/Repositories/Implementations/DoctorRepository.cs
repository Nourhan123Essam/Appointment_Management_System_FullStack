using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Infrastructure.Repositories.Interfaces;

namespace Appointment_System.Infrastructure.Repositories.Implementations
{
    public class DoctorRepository: IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddDoctorAsync(ApplicationUser doctor)
        {
            _context.Users.Add(doctor);
        }

        public async Task AddAvailabilityAsync(List<DoctorAvailability> availabilities)
        {
            _context.DoctorAvailabilities.AddRange(availabilities);
        }

        public async Task AddQualificationsAsync(List<DoctorQualification> qualifications)
        {
            _context.DoctorQualifications.AddRange(qualifications);
        }
    }
}
