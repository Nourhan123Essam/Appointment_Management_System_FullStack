using Appointment_System.Application.DTOs.Appointment;
using Appointment_System.Application.DTOs.Patient;

namespace Appointment_System.Application.Services.Interfaces
{
    public interface IPatientService
    {
        Task<PagedResult<PatientDto>> GetPatientsAsync(PatientQueryParams queryParams);
        Task<PatientDto?> GetPatientByIdAsync(string patientId);
        Task DeletePatientAsync(string patientId);
        Task<List<AppointmentDto>> GetPatientAppointmentsAsync(string patientId);
    }

}
