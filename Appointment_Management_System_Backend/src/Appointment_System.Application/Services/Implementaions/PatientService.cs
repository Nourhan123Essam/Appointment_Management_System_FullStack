using Appointment_System.Application.DTOs.Appointment;
using Appointment_System.Application.DTOs.Patient;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            var query = _repository.GetAllPatientsQueryable();

            // Filtering by name or email
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                query = query.Where(p =>
                    p.FullName.Contains(queryParams.Search) ||
                    p.Email.Contains(queryParams.Search));
            }

            // Sorting by selected field
            query = queryParams.SortBy?.ToLower() switch
            {
                "fullname" => queryParams.IsDescending ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
                "email" => queryParams.IsDescending ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
                _ => query.OrderBy(p => p.FullName) // Default sort
            };

            // Project to DTO before pagination to avoid selecting unnecessary fields
            var projectedQuery = query.Select(p => new PatientDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email
            });

            var totalCount = await projectedQuery.CountAsync();

            var items = await projectedQuery
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return new PagedResult<PatientDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
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
