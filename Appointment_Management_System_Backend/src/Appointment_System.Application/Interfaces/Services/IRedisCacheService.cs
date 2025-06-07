using Appointment_System.Application.DTOs.Doctor;

namespace Appointment_System.Application.Interfaces.Services
{
    public interface IRedisCacheService
    {
        // Basic Doctor data caching
        Task<List<DoctorBasicDto>?> GetAllDoctorsAsync();
        Task SetAllDoctorsAsync(List<DoctorBasicDto> doctors);
        Task RemoveDoctorsCacheAsync();

        Task<List<DoctorBasicDto>?> GetTop5DoctorsAsync();
        Task SetTop5DoctorsAsync(List<DoctorBasicDto> topDoctors);
    }

}
