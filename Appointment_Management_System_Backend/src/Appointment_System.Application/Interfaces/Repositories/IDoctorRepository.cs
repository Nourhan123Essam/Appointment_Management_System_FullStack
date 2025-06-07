
using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IDoctorRepository
    {
        Task<Result<Doctor>> CreateDoctorWithUserAsync(CreateDoctorDto dto, string password);
        Task<DoctorBasicDto> GetForCacheByIdAsync(int doctorId); // single item for cache
        Task<List<DoctorBasicDto>> GetAllForCacheAsync(); // all cacheable doctors

        Task UpdateBasicAsync(Doctor doctor); // Basic data (no translations)

        Task UpdateTranslationAsync(DoctorTranslation translation);

        Task DeleteAsync(int doctorId); // clean delete

        Task<Doctor?> GetDoctorByIdAsync(int doctorId);
        Task<DoctorTranslation?> GetDoctorTranslationByIdAsync(int Id);

    }

}
