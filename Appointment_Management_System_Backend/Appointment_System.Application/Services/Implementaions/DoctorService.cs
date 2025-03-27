using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Infrastructure.Repositories.Implementations;
using Appointment_System.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Appointment_System.Application.Services.Implementaions
{
    public class DoctorService: IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DoctorService(
            IDoctorRepository doctorRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _doctorRepository = doctorRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<bool> CreateDoctorAsync(DoctorCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️- Create a new ApplicationUser
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
                if (!result.Succeeded) return false;

                // 2️- Assign the "Doctor" role
                if (!await _roleManager.RoleExistsAsync("Doctor"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                }
                await _userManager.AddToRoleAsync(doctor, "Doctor");

                // 3️- Save Availability
                var availabilities = dto.Availability.Select(a => new DoctorAvailability
                {
                    DoctorId = doctor.Id,
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                }).ToList();
                await _doctorRepository.AddAvailabilityAsync(availabilities);

                // 4️- Save Qualifications
                var qualifications = dto.Qualifications.Select(q => new DoctorQualification
                {
                    DoctorId = doctor.Id,
                    QualificationName = q.QualificationName,
                    IssuingInstitution = q.IssuingInstitution,
                    YearEarned = q.YearEarned
                }).ToList();
                await _doctorRepository.AddQualificationsAsync(qualifications);

                // 5️- Commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
