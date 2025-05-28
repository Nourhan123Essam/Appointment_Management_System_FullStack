using Appointment_System.Application.DTOs.Office;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IOfficeRepository
    {
        // === Office Core ===
        Task<Office?> GetByIdAsync(int id);
        Task<List<Office>> GetAllAsync(); // Admin or availability forms
        Task AddAsync(Office office);     // With all translations
        Task DeleteByIdAsync(int id);     // Soft delete by ID
        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync();           // For Redis cache

        // === Office Translations ===
        Task<OfficeTranslation?> GetTranslationAsync(int officeId, string language);
        Task<List<OfficeTranslation>> GetTranslationsByOfficeIdAsync(int officeId); // Admin full view
        void UpdateTranslation(OfficeTranslation translation); // Update one language
        Task<List<OfficeWithTranslationsDto>> GetAllWithTranslationsAsync();
    }

}
