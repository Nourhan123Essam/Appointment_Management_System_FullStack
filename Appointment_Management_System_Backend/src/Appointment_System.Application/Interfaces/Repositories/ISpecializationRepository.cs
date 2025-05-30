using Appointment_System.Application.DTOs.Specialization;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface ISpecializationRepository
    {
        // === Core Specialization ===
        Task<Specialization?> GetByIdAsync(int id);                // Full entity with navs if needed
        Task<int> CountAsync();                                    // Total count (for dashboard status)
        Task<bool> ExistsAsync(int id);
        Task AddAsync(Specialization specialization);              // With translations
        Task DeleteByIdAsync(int id);                              // Soft delete

        // === Translations ===
        Task<SpecializationTranslation?> GetTranslationAsync(int specializationId, string language);
        Task<List<SpecializationTranslation>> GetTranslationsBySpecializationIdAsync(int specializationId);
        void UpdateTranslation(SpecializationTranslation translation);

        // === Dashboard Display ===
        Task<List<SpecializationWithTranslationsDto>> GetAllWithTranslationsAsync();

        // === Home Page Display ===
        Task<List<SpecializationWithDoctorCountDto>> GetWithDoctorCountAsync(string language);
    }

}
