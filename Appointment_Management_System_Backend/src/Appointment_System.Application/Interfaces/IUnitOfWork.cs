using Appointment_System.Application.Interfaces.Repositories;

namespace Appointment_System.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IDoctorRepository DoctorRepository { get; }
        IPatientRepository PatientRepository { get; }
        IAuthenticationRepository Authentication { get; }
        IDoctorAvailabilityRepository AvailabilityRepository { get; }
        IDoctorQualificationRepository QualificationRepository { get; }
        IOfficeRepository OfficeRepository { get; }
        ISpecializationRepository SpecializationRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
