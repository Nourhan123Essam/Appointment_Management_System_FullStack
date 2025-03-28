using Appointment_System.Application.DTOs.Doctor;

namespace Appointment_System.Application.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<bool> CreateDoctorAsync(DoctorCreateDto dto);
        Task<List<DoctorDto>> GetAllDoctorsAsync();
        Task<bool> UpdateDoctorAsync(string doctorId, DoctorUpdateDto dto);
        Task<bool> DeleteDoctorAsync(string doctorId);
    }
}
