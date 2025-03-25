using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Infrastructure.Repositories.Interfaces
{
    public interface IDoctorAvailabilityRepository
    {
        Task<DoctorAvailability?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorAvailability>> GetByDoctorIdAsync(string doctorId);
        Task AddAsync(DoctorAvailability availability);
        Task UpdateAsync(DoctorAvailability availability);
        Task DeleteAsync(int id);
    }

}
