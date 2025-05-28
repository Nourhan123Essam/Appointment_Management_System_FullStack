using Appointment_System.Domain.Entities;
using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Application.DTOs.Office
{
    public record CreateOfficeDto
    {
        public List<OfficeTranslationDto> Translations { get; init; } = new();

        public Domain.Entities.Office ToEntity()
        {
            return new Domain.Entities.Office
            {
                CreatedAt = DateTime.UtcNow,
                Translations = Translations.Select(t => new OfficeTranslation
                {
                    Language = Language.From(t.Language),
                    Name = t.Name,
                    StreetName = t.StreetName,
                    City = t.City,
                    State = t.State,
                    Country = t.Country,
                }).ToList()
            };
        }
    }
}
