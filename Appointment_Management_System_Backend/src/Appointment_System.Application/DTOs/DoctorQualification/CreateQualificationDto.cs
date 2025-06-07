namespace Appointment_System.Application.DTOs.DoctorQualification
{
    public class CreateQualificationDto
    {
        public List<QualificationTranslationDto> Translations { get; set; } = new();
        public int Date { get; set; }
    }
}
