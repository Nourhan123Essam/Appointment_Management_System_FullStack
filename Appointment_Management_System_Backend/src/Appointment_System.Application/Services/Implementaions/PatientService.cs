using Appointment_System.Application.DTOs.Appointment;
using Appointment_System.Application.DTOs.Patient;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Services.Interfaces;


namespace Appointment_System.Application.Services.Implementaions
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IDoctorRepository _doctorRepository;

        public PatientService(IPatientRepository repository, IDoctorRepository doctorRepository)
        {
            _repository = repository;
            _doctorRepository = doctorRepository;
        }

        public async Task<PagedResult<PatientDto>> GetPatientsAsync(PatientQueryParams queryParams)
        {
            return await _repository.GetPatientsAsync(queryParams);
        }

        public async Task<PatientDto?> GetPatientByIdAsync(string patientId)
        {
            var patient = await _repository.GetPatientByIdAsync(patientId);
            return patient == null ? null : new PatientDto(patient); // Safe null check
        }

        public async Task DeletePatientAsync(string patientId)
        {
            var patient = await _repository.GetPatientByIdAsync(patientId);
            if (patient != null)
            {
                await _repository.DeletePatientAsync(patient);
            }
        }

        public async Task<List<AppointmentDto>> GetPatientAppointmentsAsync(string patientId)
        {
            var appointments = await _repository.GetPatientAppointmentsAsync(patientId);

            // Collect unique doctor IDs to avoid N+1 queries
            var doctors = await _doctorRepository.GetAllDoctorsAsync();

            // Build a dictionary for fast lookup
            var doctorDict = doctors.ToDictionary(d => d.Id);

            // Map appointments to DTOs with doctor name
            var appointmentDtos = appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                AppointmentTime = a.AppointmentTime,
                DoctorName = doctorDict.TryGetValue(a.DoctorId, out var doc) ? doc.FullName : "Unknown"
            }).ToList();

            return appointmentDtos;
        }
    }


}
