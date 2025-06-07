using Appointment_System.Infrastructure.Data;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Infrastructure.Seeders;

public class OfficeTestDataSeeder
{
    private readonly ApplicationDbContext _context;

    public OfficeTestDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var offices = new List<Office>();

        for (int i = 1; i <= 10; i++)
        {
            var office = new Office
            {
                CreatedAt = DateTime.UtcNow,
                Translations = new List<OfficeTranslation>
                {
                    new OfficeTranslation
                    {
                        Language = new Domain.ValueObjects.Language("en-US"),
                        Name = $"Office {i}",
                        StreetName = $"Street {i}",
                        City = "CityX",
                        State = "StateY",
                        Country = "CountryZ"
                    },
                    new OfficeTranslation
                    {
                        Language = new Domain.ValueObjects.Language("ar-EG"),
                        Name = $"مكتب {i}",
                        StreetName = $"شارع {i}",
                        City = "المدينة X",
                        State = "الولاية Y",
                        Country = "البلد Z"
                    }
                }
            };

            offices.Add(office);
        }

        _context.Offices.AddRange(offices);
        await _context.SaveChangesAsync();
    }
}
