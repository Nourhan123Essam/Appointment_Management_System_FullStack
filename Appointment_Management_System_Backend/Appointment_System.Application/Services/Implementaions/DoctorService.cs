using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
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

        // Get Dcotor by Id 
        public async Task<DoctorDto?> GetDoctorByIdAsync(string doctorId)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            return doctor == null ? null : new DoctorDto(doctor);
        }



        // Create Docotor
        public async Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto dto)
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
                if (!result.Succeeded) return null;

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


                // 6️- Fetch and return doctor with full details
                return await GetDoctorByIdAsync(doctor.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }


        // Get All Doctors basic Data
        public async Task<List<DoctorsBasicDataDto>> GetAllDoctorsBasicDataAsync()
        {
            var doctors = await _doctorRepository.GetAllDoctorsBasicDataAsync();

            return doctors.Select(d => new DoctorsBasicDataDto(
                Id: d.Id,
                FullName: d.FullName,
                YearsOfExperience: d.YearsOfExperience,
                Specialization: d.Specialization,
                LicenseNumber: d.LicenseNumber,
                ConsultationFee: d.ConsultationFee,
                WorkplaceType: d.WorkplaceType
            )).ToList();
        }

        // Get All Doctors
        public async Task<List<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetAllDoctorsAsync();

            return doctors.Select(d => new DoctorDto(d)).ToList();
        }

        //Update Doctor
        public async Task<bool> UpdateDoctorAsync(string doctorId, DoctorUpdateDto dto)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return false;

            // Update only doctor details
            doctor.FullName = dto.FullName;
            doctor.YearsOfExperience = dto.YearsOfExperience;
            doctor.Specialization = dto.Specialization;
            doctor.LicenseNumber = dto.LicenseNumber;
            doctor.ConsultationFee = dto.ConsultationFee;
            doctor.WorkplaceType = dto.WorkplaceType;

            return await _doctorRepository.UpdateDoctorAsync(doctor);
        }

        // Delete Doctor
        public async Task<bool> DeleteDoctorAsync(string doctorId)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return false;

            return await _doctorRepository.DeleteDoctorAsync(doctor);
        }
    }
}
