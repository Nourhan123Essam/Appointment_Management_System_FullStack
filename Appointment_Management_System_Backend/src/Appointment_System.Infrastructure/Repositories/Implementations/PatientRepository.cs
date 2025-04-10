using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Repositories.Implementations
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
        public IQueryable<ApplicationUser> GetAllPatientsQueryable()
        {
            var patientRoleId =  _context.Roles
                .Where(r => r.Name == "Patient")
                .Select(r => r.Id)
                .First();

            return  _context.Users.AsNoTracking()
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId == patientRoleId));
        }


        public async Task<ApplicationUser?> GetPatientByIdAsync(string patientId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == patientId);
        }

        public async Task DeletePatientAsync(ApplicationUser patient)
        {
            _context.Users.Remove(patient);
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
    }


}
