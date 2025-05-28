using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces;
using Appointment_System.Infrastructure.Data;

namespace Appointment_System.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IDoctorRepository Doctors { get; }
        public IPatientRepository Patients { get; }

        public IAuthenticationRepository Authentication { get; }

        public IDoctorAvailabilityRepository AvailabilityRepository { get; }

        public IDoctorQualificationRepository QualificationRepository { get; }
        public IOfficeRepository OfficeRepository { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IDoctorRepository doctorRepo,
            IPatientRepository patientRepo,
            IAuthenticationRepository authenticationRepo,
            IDoctorAvailabilityRepository doctorAvailabilityRepo,
            IDoctorQualificationRepository doctorQualificationRepo,
            IOfficeRepository officeRepository
        ){
            _context = context;
            Doctors = doctorRepo;
            Patients = patientRepo;
            Authentication = authenticationRepo;
            AvailabilityRepository = doctorAvailabilityRepo;
            QualificationRepository = doctorQualificationRepo;
            OfficeRepository = officeRepository;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }

}
