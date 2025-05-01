
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IDoctorAvailabilityRepository
    {
        Task<DoctorAvailability?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorAvailability>> GetByDoctorIdAsync(string doctorId);
        Task<Guid> AddAsync(DoctorAvailability availability);
        Task UpdateAsync(DoctorAvailability availability);
        Task DeleteAsync(int id);
    }

}
