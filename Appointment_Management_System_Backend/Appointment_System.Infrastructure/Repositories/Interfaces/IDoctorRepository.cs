using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;

namespace Appointment_System.Infrastructure.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task AddDoctorAsync(ApplicationUser doctor);
        Task AddAvailabilityAsync(List<DoctorAvailability> availabilities);
        Task AddQualificationsAsync(List<DoctorQualification> qualifications);
    }
}
