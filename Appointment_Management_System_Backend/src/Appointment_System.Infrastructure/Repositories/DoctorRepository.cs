using System;
using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DoctorRepository(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Get Doctor by Id
        public async Task<User?> GetDoctorByIdAsync(string doctorId)
        {
            return (await _context.Users
                .Include(d => d.Availabilities)
                .Include(d => d.Qualifications)
                .FirstOrDefaultAsync(d => d.Id == doctorId)).ToDomain();
        }


        // Create Doctor
        public async Task AddDoctorAsync(User doctor)
        {
            _context.Users.Add(new ApplicationUser(doctor));
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
        public async Task<List<User>> GetAllDoctorsBasicDataAsync()
        {
            // Get doctors using UserManager
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            return doctors.Select(d => d.ToDomain()).ToList();
        }

        // Get all Doctors
        public async Task<List<User>> GetAllDoctorsAsync()
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

            return doctorsWithDetails.Select(d => d.ToDomain()).ToList();
        }

        // Update Doctor
        public async Task<bool> UpdateDoctorAsync(User doctor)
        {
            //var Doctor = await GetDoctorByIdAsync(doctor.Id);
            //if (Doctor == null) { return false; }
            var updatedDoctor = new ApplicationUser(doctor);
            _context.Users.Update(updatedDoctor);
            return (await _context.SaveChangesAsync()) > 0;
        }

            //updatedDoctor.Id = doctor.Id;
            //updatedDoctor.Email = doctor.Email;
            //updatedDoctor.PhoneNumber = doctor.PhoneNumber;
            //updatedDoctor.FullName = doctor.FullName;
            //updatedDoctor.UserName = doctor.Email;
            //updatedDoctor.LicenseNumber = doctor.LicenseNumber;
            //updatedDoctor.Specialization = doctor.Specialization;
            //updatedDoctor.TotalRatingScore = doctor.TotalRatingScore;
            //updatedDoctor.TotalRatingsGiven = doctor.TotalRatingsGiven;
            //updatedDoctor.WorkplaceType = doctor.WorkplaceType;
            

        // Delete Doctor
        public async Task<bool> DeleteDoctorAsync(User doctor)
        {
            _context.Users.Remove(new ApplicationUser(doctor));
            return await _context.SaveChangesAsync() > 0;
        }


        // create with trasaction
        public async Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var doctor = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    YearsOfExperience = dto.YearsOfExperience,
                    Specialization = dto.Specialization,
                    LicenseNumber = dto.LicenseNumber,
                    ConsultationFee = dto.ConsultationFee,
                    WorkplaceType = dto.WorkplaceType
                };

                var result = await _userManager.CreateAsync(doctor, dto.Password);
                if (!result.Succeeded) return null;

                if (!await _roleManager.RoleExistsAsync("Doctor"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                }
                await _userManager.AddToRoleAsync(doctor, "Doctor");

                var availabilities = dto.Availabilities.Select(a => new DoctorAvailability
                {
                    DoctorId = doctor.Id,
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                }).ToList();
                await _context.DoctorAvailabilities.AddRangeAsync(availabilities);

                var qualifications = dto.Qualifications.Select(q => new DoctorQualification
                {
                    DoctorId = doctor.Id,
                    QualificationName = q.QualificationName,
                    IssuingInstitution = q.IssuingInstitution,
                    YearEarned = q.YearEarned
                }).ToList();
                await _context.DoctorQualifications.AddRangeAsync(qualifications);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Map doctor to DoctorDto or fetch full details if needed
                return new DoctorDto(doctor.ToDomain()); // Or fetch from DB with includes if needed
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

    }
}
