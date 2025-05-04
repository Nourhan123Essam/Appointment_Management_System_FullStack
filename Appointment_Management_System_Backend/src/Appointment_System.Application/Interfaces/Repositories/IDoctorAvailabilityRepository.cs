
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IDoctorAvailabilityRepository
    {
        Task<Availability?> GetByIdAsync(int id);
        Task<IEnumerable<Availability>> GetByDoctorIdAsync(int doctorId);
        Task<int> AddAsync(Availability availability);
        Task UpdateAsync(Availability availability);
        Task DeleteAsync(int id);
    }

}
