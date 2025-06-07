using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Domain.Entities
{
    public class SpecializationTranslation
    {
        public int Id { get; set; }

        public int SpecializationId { get; set; }
        public Language LanguageValue { get; set; } = null!; // Use ISO code (e.g., "en-US", "ar-EG")
        public string Name { get; set; } = null!;

        // Navigation
        public Specialization Specialization { get; set; } = new();
    }

}
