namespace Appointment_System.Application.DTOs.Office
{
    public class OfficeTranslationDto
    {
        public int Id { get; set; }
        public int OfficeId { get; set; }

        public string Language { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string StreetName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
    }

}
