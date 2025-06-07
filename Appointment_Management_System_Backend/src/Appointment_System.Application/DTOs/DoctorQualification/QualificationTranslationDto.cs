namespace Appointment_System.Application.DTOs.DoctorQualification
{
    public class QualificationTranslationDto
    {
        public int Id { get; set; }
        public int QualificationId { get; set; }

        public string Language { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Institution { get; set; } = null!;
    }
}
