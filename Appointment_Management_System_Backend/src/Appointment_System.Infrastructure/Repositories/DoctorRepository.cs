using System.Security.Principal;
using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;
using Appointment_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdentityRepository _identity;

        public DoctorRepository(ApplicationDbContext context, IIdentityRepository identityRepository)
        {
            _context = context;
            _identity = identityRepository;
        }

        public async Task<Doctor> GetDoctorByIdAsync(int doctorId)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        }

        public async Task<DoctorTranslation?> GetDoctorTranslationByIdAsync(int Id)
        {
            return await _context.DoctorTranslations.FirstOrDefaultAsync(d => d.Id == Id);
        }

        public async Task<Result<Doctor>> CreateDoctorWithUserAsync(CreateDoctorDto dto, string password)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var userId = await _identity.CreateUserAsync(dto.Email, password);
                if (userId is null)
                    return Result<Doctor>.Fail("Failed to create user.");

                var roleAssigned = await _identity.AssignRoleAsync(userId, "Doctor");
                if (!roleAssigned)
                    return Result<Doctor>.Fail("Failed to assign Doctor role.");

                var doctor = dto.ToEntity(userId);
                await _context.Doctors.AddAsync(doctor);
                await _context.SaveChangesAsync();

                // Reload the doctor with required navigation properties
                var fullDoctor = await _context.Doctors
                    .Include(d => d.DoctorSpecializations)
                        .ThenInclude(ds => ds.Specialization)
                            .ThenInclude(s => s.Translations)
                    .Include(d => d.Translations)
                    .FirstAsync(d => d.Id == doctor.Id);

                await transaction.CommitAsync();
                
                return Result<Doctor>.Success(fullDoctor);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result<Doctor>.Fail("An error occurred while creating the doctor: " + ex.Message);
            }
        }


        public Task UpdateBasicAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor); // good practice if entity is tracked
            return Task.CompletedTask;
        }

        public async Task UpdateTranslationAsync(DoctorTranslation translation)
        {
            var existing = await _context.DoctorTranslations
                .FirstOrDefaultAsync(t => t.DoctorId == translation.DoctorId && t.Language == translation.Language);

            if (existing is not null)
            {
                existing.FirstName = translation.FirstName;
                existing.LastName = translation.LastName;
                existing.Bio = translation.Bio;
            }
            else
            {
                await _context.DoctorTranslations.AddAsync(translation);
            }
        }

        public async Task DeleteAsync(int doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor is not null)
            {
                doctor.IsDeleted = true;
            }
        }

        public async Task<List<DoctorBasicDto>> GetAllForCacheAsync()
        {
            var doctors = await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Translations)
                .Include(d => d.DoctorSpecializations)
                    .ThenInclude(ds => ds.Specialization)
                        .ThenInclude(s => s.Translations)
                .ToListAsync();

            return doctors.Select(DoctorBasicDto.FromEntity).ToList();
        }

        public async Task<DoctorBasicDto?> GetForCacheByIdAsync(int doctorId)
        {
            var doctor = await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Translations)
                .Include(d => d.DoctorSpecializations)
                    .ThenInclude(ds => ds.Specialization)
                        .ThenInclude(s => s.Translations)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            return doctor is not null ? DoctorBasicDto.FromEntity(doctor) : null;
        }
    }

}
