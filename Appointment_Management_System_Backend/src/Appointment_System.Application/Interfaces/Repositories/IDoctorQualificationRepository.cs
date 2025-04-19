using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IDoctorQualificationRepository
    {
        Task<DoctorQualification?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorQualification>> GetByDoctorIdAsync(string doctorId);
        Task<int> AddAsync(DoctorQualification qualification);
        Task UpdateAsync(DoctorQualification qualification);
        Task DeleteAsync(int id);
    }

}
