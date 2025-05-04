using Appointment_System.Application.DTOs.Patient;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientByIdAsync(int patientId);
        Task DeletePatientAsync(Patient patient); 
        Task AddAsync(Patient patient);
        Task<List<Appointment>> GetPatientAppointmentsAsync(int patientId);
        Task<PagedResult<PatientDto>> GetPatientsAsync(PatientQueryParams queryParams);

    }


}
