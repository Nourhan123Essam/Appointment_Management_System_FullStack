using Appointment_System.Domain.Entities;

namespace Appointment_System.Infrastructure.Repositories.Interfaces
{
    public interface IDoctorQualificationRepository
    {
        Task<DoctorQualification?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorQualification>> GetByDoctorIdAsync(string doctorId);
        Task AddAsync(DoctorQualification qualification);
        Task UpdateAsync(DoctorQualification qualification);
        Task DeleteAsync(int id);
    }

}
