using Appointment_System.Application.DTOs.Patient;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IPatientRepository
    {
        IQueryable<User> GetAllPatientsQueryable(); // for filtering, sorting, pagination
        Task<User?> GetPatientByIdAsync(string patientId);
        Task DeletePatientAsync(User patient);
        Task<List<Appointment>> GetPatientAppointmentsAsync(string patientId);
        Task<PagedResult<PatientDto>> GetPatientsAsync(PatientQueryParams queryParams);

    }


}
