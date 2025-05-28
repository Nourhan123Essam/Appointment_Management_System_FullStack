namespace Appointment_System.Application.DTOs.Office
{
    public record OfficeWithTranslationsDto
    {
        public int Id { get; init; }
        public List<OfficeTranslationDto> Translations { get; init; } = new();
    }

}
