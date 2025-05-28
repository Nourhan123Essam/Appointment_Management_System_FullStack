using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.DTOs.Office
{
    public record OfficeDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string StreetName { get; init; } = null!;
        public string City { get; init; } = null!;
        public string State { get; init; } = null!;
        public string Country { get; init; } = null!;

        public static OfficeDto FromEntity(Domain.Entities.Office office, OfficeTranslation translation)
        {
            return new OfficeDto
            {
                Id = office.Id,
                Name = translation.Name,
                StreetName = translation.StreetName,
                City = translation.City,
                State = translation.State,
                Country = translation.Country
            };
        }
    }

}
