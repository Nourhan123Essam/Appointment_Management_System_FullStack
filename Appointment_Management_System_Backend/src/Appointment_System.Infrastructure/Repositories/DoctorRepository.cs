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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DoctorRepository(ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Get Doctor by Id
        public async Task<Doctor?> GetDoctorByIdAsync(int doctorId)
        {
            return (await _context.Doctors
                .Include(d => d.Availabilities)
                .Include(d => d.Qualifications)
                .FirstOrDefaultAsync(d => d.Id == doctorId));
        }


        // Create Doctor
        public async Task AddDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
        }

        public async Task AddAvailabilityAsync(List<Availability> availabilities)
        {
            _context.DoctorAvailabilities.AddRange(availabilities);
        }

        public async Task AddQualificationsAsync(List<Qualification> qualifications)
        {
            _context.DoctorQualifications.AddRange(qualifications);
        }

        // Get all Doctors basic Data
        public async Task<List<Doctor>> GetAllDoctorsBasicDataAsync()
        {
            // Get doctors using UserManager
            var doctors = await _context.Doctors.ToListAsync();
            return doctors;
        }

        // Get all Doctors
        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            // Get doctors using UserManager
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");

            // Fetch qualifications & availabilities for the doctors
            var doctorIds = doctors.Select(d => d.Id).ToList();

            var doctorsWithDetails = await _context.Doctors
                .Where(d => doctorIds.Contains(d.UserId))
                .Include(d => d.Qualifications)  // Include qualifications
                .Include(d => d.Availabilities)  // Include availabilities
                .AsNoTracking()
                .ToListAsync();

            return doctorsWithDetails;
        }

        // Update Doctor
        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            return (await _context.SaveChangesAsync()) > 0;
        }            

        // Delete Doctor
        public async Task<bool> DeleteDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Remove(doctor);
            return await _context.SaveChangesAsync() > 0;
        }


        // create with trasaction
        public async Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto dto, string password)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new IdentityUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                };

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded) return null;

                var doctor = new Doctor
                {
                    FirstName = dto.FirstName,
                    Email = dto.Email,
                    LastName = dto.LastName,
                    YearsOfExperience = dto.YearsOfExperience,
                    InitialFee = dto.InitialFee,
                    FollowUpFee = dto.FollowUpFee,
                    MaxFollowUps = dto.MaxFollowUps,
                    Bio = dto.Bio
                };

                var res = await _context.Doctors.AddAsync(doctor);
                if (res == null) return null;

                if (!await _roleManager.RoleExistsAsync("Doctor"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                }
                await _userManager.AddToRoleAsync(user, "Doctor");

                var availabilities = dto.Availabilities.Select(a => new Availability
                {
                    DoctorId = doctor.Id,
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                }).ToList();
                await _context.DoctorAvailabilities.AddRangeAsync(availabilities);

                var qualifications = dto.Qualifications.Select(q => new Qualification
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
                return new DoctorDto(doctor); // Or fetch from DB with includes if needed
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

    }
}
