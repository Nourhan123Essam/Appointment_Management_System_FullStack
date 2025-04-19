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
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // for filtering, sorting, pagination
        public IQueryable<User> GetAllPatientsQueryable()
        {
            var patientRoleId = _context.Roles
                .Where(r => r.Name == "Patient")
                .Select(r => r.Id)
                .First();

            var patients = _context.Users.AsNoTracking()
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId == patientRoleId));

            return patients.Select(p => p.ToDomain());
        }


        public async Task<User?> GetPatientByIdAsync(string patientId)
        {
            return (await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == patientId)).ToDomain();
        }

        public async Task DeletePatientAsync(User patient)
        {
            var user = new ApplicationUser(patient);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Appointment>> GetPatientAppointmentsAsync(string patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentTime)
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
