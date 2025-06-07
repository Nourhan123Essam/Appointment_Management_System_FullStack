using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Domain.Entities
{
    public class QualificationTranslation
    {
        public int Id { get; set; }
        public int QualificationId { get; set; }
        public Language Language { get; set; } = null!;

        public string QualificationName { get; set; } = null!; // Name of the qualification (e.g., MD, MSc, etc.)
        public string IssuingInstitution { get; set; } = null!; // Example: Harvard Medical School

        public Qualification Qualification { get; set; } = new();
    }

}
