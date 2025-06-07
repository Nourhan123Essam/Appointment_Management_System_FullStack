using Appointment_System.Infrastructure.Data;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Infrastructure.Seeders;

public class SpecializationTestDataSeeder
{
    private readonly ApplicationDbContext _context;

    public SpecializationTestDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var specializations = new List<Specialization>();

        for (int i = 1; i <= 10; i++)
        {
            var specialization = new Specialization
            {
                CreatedAt = DateTime.UtcNow,
                Translations = new List<SpecializationTranslation>
                {
                    new SpecializationTranslation
                    {
                        LanguageValue = new Domain.ValueObjects.Language("en-US"),
                        Name = $"Specialization {i}"
                    },
                    new SpecializationTranslation
                    {
                        LanguageValue = new Domain.ValueObjects.Language("ar-EG"),
                        Name = $"تخصص {i}"
                    }
                }
            }; 

            specializations.Add(specialization);
        }

        _context.Specializations.AddRange(specializations);
        await _context.SaveChangesAsync();
    }
}
