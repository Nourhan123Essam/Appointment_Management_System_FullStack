using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IDoctorQualificationRepository
    {
        Task<Qualification?> GetByIdAsync(int id);
        Task<IEnumerable<Qualification>> GetByDoctorIdAsync(int doctorId);
        Task<int> AddAsync(Qualification qualification);
        Task UpdateAsync(Qualification qualification);
        Task DeleteAsync(int id);
    }

}
