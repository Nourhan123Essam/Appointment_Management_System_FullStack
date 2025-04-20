using Appointment_System.Application.DTOs.Appointment;
using Appointment_System.Application.DTOs.Patient;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Services.Interfaces;


namespace Appointment_System.Application.Services.Implementaions
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientService(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<PatientDto>> GetPatientsAsync(PatientQueryParams queryParams)
        {
            return await _unitOfWork.Patients.GetPatientsAsync(queryParams);
        }

        public async Task<PatientDto?> GetPatientByIdAsync(string patientId)
        {
            var patient = await _unitOfWork.Patients.GetPatientByIdAsync(patientId);
            return patient == null ? null : new PatientDto(patient); // Safe null check
        }

        public async Task DeletePatientAsync(string patientId)
        {
            var patient = await _unitOfWork.Patients.GetPatientByIdAsync(patientId);
            if (patient != null)
            {
                await _unitOfWork.Patients.DeletePatientAsync(patient);
            }
        }

        public async Task<List<AppointmentDto>> GetPatientAppointmentsAsync(string patientId)
        {
            var appointments = await _unitOfWork.Patients.GetPatientAppointmentsAsync(patientId);

            // Collect unique doctor IDs to avoid N+1 queries
            var doctors = await _unitOfWork.Doctors.GetAllDoctorsAsync();

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
