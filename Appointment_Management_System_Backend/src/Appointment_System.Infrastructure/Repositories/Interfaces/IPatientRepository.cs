using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;

namespace Appointment_System.Infrastructure.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        IQueryable<ApplicationUser> GetAllPatientsQueryable(); // for filtering, sorting, pagination
        Task<ApplicationUser?> GetPatientByIdAsync(string patientId);
        Task DeletePatientAsync(ApplicationUser patient);
        Task<List<Appointment>> GetPatientAppointmentsAsync(string patientId);
    }
    

}
