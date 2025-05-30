namespace Appointment_System.Application.DTOs.Specialization
{
    public class SpecializationWithTranslationsDto
    {
        public int Id { get; set; }
        public List<SpecializationTranslationDto> Translations { get; set; } = new();
    }
}
