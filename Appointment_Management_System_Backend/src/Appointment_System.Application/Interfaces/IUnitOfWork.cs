using Appointment_System.Application.Interfaces.Repositories;

namespace Appointment_System.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IDoctorRepository Doctors { get; }
        IPatientRepository Patients { get; }
        IAuthenticationRepository Authentication { get; }
        IDoctorAvailabilityRepository AvailabilityRepository { get; }
        IDoctorQualificationRepository QualificationRepository { get; }
        IOfficeRepository OfficeRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
