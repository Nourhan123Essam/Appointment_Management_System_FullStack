using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces;
using Appointment_System.Infrastructure.Data;

namespace Appointment_System.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IDoctorRepository DoctorRepository { get; }
        public IPatientRepository PatientRepository { get; }

        public IAuthenticationRepository Authentication { get; }

        public IDoctorAvailabilityRepository AvailabilityRepository { get; }

        public IDoctorQualificationRepository QualificationRepository { get; }
        public IOfficeRepository OfficeRepository { get; }
        public ISpecializationRepository SpecializationRepository { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IDoctorRepository doctorRepo,
            IPatientRepository patientRepo,
            IAuthenticationRepository authenticationRepo,
            IDoctorAvailabilityRepository doctorAvailabilityRepo,
            IDoctorQualificationRepository doctorQualificationRepo,
            IOfficeRepository officeRepository,
            ISpecializationRepository specializationRepository
        ){
            _context = context;
            DoctorRepository = doctorRepo;
            PatientRepository = patientRepo;
            Authentication = authenticationRepo;
            AvailabilityRepository = doctorAvailabilityRepo;
            QualificationRepository = doctorQualificationRepo;
            OfficeRepository = officeRepository;
            SpecializationRepository = specializationRepository;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }

}
