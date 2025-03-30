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
        Task<List<ApplicationUser>> GetAllDoctorsAsync();
        Task<List<ApplicationUser>> GetAllDoctorsBasicDataAsync();
        Task<bool> UpdateDoctorAsync(ApplicationUser doctor);
        Task<ApplicationUser> GetDoctorByIdAsync(string id);
        Task<bool> DeleteDoctorAsync(ApplicationUser doctor);
    }
}
