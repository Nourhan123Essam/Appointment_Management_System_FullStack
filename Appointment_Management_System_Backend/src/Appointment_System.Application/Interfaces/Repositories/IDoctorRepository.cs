
using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IDoctorRepository
    {
        Task AddDoctorAsync(Doctor doctor);
        Task AddAvailabilityAsync(List<Availability> availabilities);
        Task AddQualificationsAsync(List<Qualification> qualifications);
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<List<Doctor>> GetAllDoctorsBasicDataAsync();
        Task<bool> UpdateDoctorAsync(Doctor doctor);
        Task<Doctor> GetDoctorByIdAsync(int id);
        Task<bool> DeleteDoctorAsync(Doctor doctor);
        Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto dto, string password);
    }
}
