
using Appointment_System.Application.Interfaces.Repositories;

namespace Appointment_System.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IDoctorRepository Doctors { get; }
        IPatientRepository Patients { get; }
        IDoctorAvailabilityRepository AvailabilityRepository { get; }
        IDoctorQualificationRepository doctorQualificationRepository { get; }
    }
}
