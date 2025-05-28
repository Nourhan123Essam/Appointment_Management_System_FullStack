using Appointment_System.Application.DTOs.Office;
using System;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Infrastructure.Repositories
{
    // NOTE:
    // Soft delete filtering (IsDeleted = false) is applied globally using HasQueryFilter()
    // in the Office entity configuration class. Therefore, repository methods here do not
    // need to manually filter by IsDeleted — it's automatically excluded from queries.
    public class OfficeRepository : IOfficeRepository
    {
        private readonly ApplicationDbContext _context;

        public OfficeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Office?> GetByIdAsync(int id)
        {
            return await _context.Offices
                .Include(o => o.Translations)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Office>> GetAllAsync()
        {
            return await _context.Offices
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Office office)
        {
            await _context.Offices.AddAsync(office);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var office = await _context.Offices.FindAsync(id);
            if (office != null)
            {
                office.IsDeleted = true;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Offices
                .AnyAsync(o => o.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Offices.CountAsync();
        }

        public async Task<OfficeTranslation?> GetTranslationAsync(int officeId, string language)
        {
            return await _context.OfficeTranslations
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.OfficeId == officeId && t.Language.Value == language);
        }

        public async Task<List<OfficeTranslation>> GetTranslationsByOfficeIdAsync(int officeId)
        {
            return await _context.OfficeTranslations
                .Where(t => t.OfficeId == officeId)
                .AsNoTracking()
                .ToListAsync();
        }

        public void UpdateTranslation(OfficeTranslation translation)
        {
            _context.OfficeTranslations.Update(translation);
        }

        public async Task<List<OfficeWithTranslationsDto>> GetAllWithTranslationsAsync()
        {
            return await _context.Offices
                .Where(o => !o.IsDeleted)
                .Include(o => o.Translations)
                .Select(o => new OfficeWithTranslationsDto
                {
                    Id = o.Id,
                    Translations = o.Translations
                        .Select(t => new OfficeTranslationDto
                        {
                            Language = t.Language.Value, // value object
                            Name = t.Name,
                            StreetName = t.StreetName,
                            City = t.City,
                            State = t.State,
                            Country = t.Country
                        })
                        .ToList()
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
