using Appointment_System.Application.DTOs.Patient;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PatientRepository(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // create patient
        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        // for filtering, sorting, pagination
        public IQueryable<Patient> GetAllPatientsQueryable()
        {
            var patientRoleId = _context.Roles
                .Where(r => r.Name == "Patient")
                .Select(r => r.Id)
                .First();

            var patients = _context.Patients.AsNoTracking();

            return patients;
        }


        public async Task<Patient?> GetPatientByIdAsync(int patientId)
        {
            return await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == patientId);
        }

        public async Task DeletePatientAsync(Patient patient)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Appointment>> GetPatientAppointmentsAsync(int patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.DateTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PagedResult<PatientDto>> GetPatientsAsync(PatientQueryParams queryParams)
        {
            var query = GetAllPatientsQueryable();

            // Filtering by name or email
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                query = query.Where(p =>
                    p.FirstName.Contains(queryParams.Search) ||
                    p.LastName.Contains(queryParams.Search) ||
                    p.Email.Contains(queryParams.Search));
            }

            // Sorting by selected field
            query = queryParams.SortBy?.ToLower() switch
            {
                "fullname" => queryParams.IsDescending ? query.OrderByDescending(p => p.FirstName + p.LastName) : query.OrderBy(p => p.FirstName + p.LastName),
                "email" => queryParams.IsDescending ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
                _ => query.OrderBy(p => p.FirstName + p.LastName) // Default sort
            };

            // Project to DTO before pagination to avoid selecting unnecessary fields
            var projectedQuery = query.Select(p => new PatientDto(p));

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

       
    } 
}
