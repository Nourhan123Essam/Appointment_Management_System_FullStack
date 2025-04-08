using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Repositories.Implementations
{
    public class DoctorRepository: IDoctorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // Get Doctor by Id
        public async Task<ApplicationUser?> GetDoctorByIdAsync(string doctorId)
        {
            return await _context.Users
                .Include(d => d.Availabilities)
                .Include(d => d.Qualifications)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
        }


        // Create Doctor
        public async Task AddDoctorAsync(ApplicationUser doctor)
        {
            _context.Users.Add(doctor);
        }

        public async Task AddAvailabilityAsync(List<DoctorAvailability> availabilities)
        {
            _context.DoctorAvailabilities.AddRange(availabilities);
        }

        public async Task AddQualificationsAsync(List<DoctorQualification> qualifications)
        {
            _context.DoctorQualifications.AddRange(qualifications);
        }

        // Get all Doctors basic Data
        public async Task<List<ApplicationUser>> GetAllDoctorsBasicDataAsync()
        {
            // Get doctors using UserManager
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            return doctors.ToList();
        }

        // Get all Doctors
        public async Task<List<ApplicationUser>> GetAllDoctorsAsync()
        {
            // Get doctors using UserManager
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");

            // Fetch qualifications & availabilities for the doctors
            var doctorIds = doctors.Select(d => d.Id).ToList();

            var doctorsWithDetails = await _context.Users
                .Where(d => doctorIds.Contains(d.Id))
                .Include(d => d.Qualifications)  // Include qualifications
                .Include(d => d.Availabilities)  // Include availabilities
                .AsNoTracking()
                .ToListAsync();

            return doctorsWithDetails;
        }

        // Update Doctor
        public async Task<bool> UpdateDoctorAsync(ApplicationUser doctor)
        {
            _context.Users.Update(doctor);
            return await _context.SaveChangesAsync() > 0;
        }

        // Delete Doctor
        public async Task<bool> DeleteDoctorAsync(ApplicationUser doctor)
        {
            _context.Users.Remove(doctor);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
