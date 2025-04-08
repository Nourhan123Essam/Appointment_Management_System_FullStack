using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Infrastructure.Data;

namespace Appointment_System.Application.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorDto?> GetDoctorByIdAsync(string doctorId);
        Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto dto);
        Task<List<DoctorDto>> GetAllDoctorsAsync();
        Task<List<DoctorsBasicDataDto>> GetAllDoctorsBasicDataAsync();
        Task<bool> UpdateDoctorAsync(string doctorId, DoctorUpdateDto dto);
        Task<bool> DeleteDoctorAsync(string doctorId);
    }
}
