using Appointment_System.Application.DTOs.Specialization;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Appointment_System.Infrastructure.Repositories
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly ApplicationDbContext _context;

        public SpecializationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // === Core ===

        public async Task<Specialization?> GetByIdAsync(int id)
        {
            return await _context.Specializations
                .Include(s => s.Translations)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Specializations.CountAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Specializations.AnyAsync(s => s.Id == id);
        }

        public async Task AddAsync(Specialization specialization)
        {
            await _context.Specializations.AddAsync(specialization);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var specialization = await _context.Specializations.FindAsync(id);
            if (specialization != null)
            {
                specialization.IsDeleted = true;
            }
        }

        // === Translations ===

        public async Task<SpecializationTranslation?> GetTranslationAsync(int specializationId, string language)
        {
            return await _context.SpecializationsTranslation
                .FirstOrDefaultAsync(t => t.SpecializationId == specializationId && t.LanguageValue.Value == language);
        }

        public async Task<List<SpecializationTranslation>> GetTranslationsBySpecializationIdAsync(int specializationId)
        {
            return await _context.SpecializationsTranslation
                .Where(t => t.SpecializationId == specializationId)
                .ToListAsync();
        }

        public void UpdateTranslation(SpecializationTranslation translation)
        {
            _context.SpecializationsTranslation.Update(translation);
        }

        // === Dashboard ===

        public async Task<List<SpecializationWithTranslationsDto>> GetAllWithTranslationsAsync()
        {
            return await _context.Specializations
                .AsNoTracking()
                .Select(s => new SpecializationWithTranslationsDto
                {
                    Id = s.Id,
                    Translations = s.Translations
                        .Select(t => new SpecializationTranslationDto
                        {
                            Language = t.LanguageValue.Value,
                            Name = t.Name,
                            Id = t.Id,
                            SpecializationId = t.SpecializationId
                        }).ToList()
                })
                .ToListAsync();
        }

        // === Home Page ===

        public async Task<List<SpecializationWithDoctorCountDto>> GetWithDoctorCountAsync(string language)
        {
            return await _context.Specializations
                .AsNoTracking()
                .Select(s => new SpecializationWithDoctorCountDto
                {
                    SpecializationId = s.Id,
                    Name = s.Translations
                        .Where(t => t.LanguageValue.Value == language)
                        .Select(t => t.Name)
                        .FirstOrDefault() ?? "[Unknown]",
                    DoctorCount = s.DoctorSpecializations.Count
                })
                .OrderByDescending(x => x.DoctorCount)
                .ToListAsync();
        }
    }

}
