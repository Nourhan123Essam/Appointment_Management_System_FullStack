using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Domain.Entities
{
    public class OfficeTranslation
    {
        public int Id { get; set; }
        public int OfficeId { get; set; }
        public Office Office { get; set; } = null!;

        public Language Language { get; set; } = null!; 

        public string Name { get; set; } = null!;
        public string StreetName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
    }

}
