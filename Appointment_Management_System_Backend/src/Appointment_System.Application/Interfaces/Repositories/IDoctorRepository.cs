
using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IDoctorRepository
    {
        Task AddDoctorAsync(User doctor);
        Task AddAvailabilityAsync(List<DoctorAvailability> availabilities);
        Task AddQualificationsAsync(List<DoctorQualification> qualifications);
        Task<List<User>> GetAllDoctorsAsync();
        Task<List<User>> GetAllDoctorsBasicDataAsync();
        Task<bool> UpdateDoctorAsync(User doctor);
        Task<User> GetDoctorByIdAsync(string id);
        Task<bool> DeleteDoctorAsync(User doctor);
        Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto dto);
    }
}
