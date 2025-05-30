namespace Appointment_System.Application.DTOs.Specialization
{
    public class SpecializationTranslationDto
    {
        public int Id { get; set; }
        public int SpecializationId { get; set; }
        public string Language { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
