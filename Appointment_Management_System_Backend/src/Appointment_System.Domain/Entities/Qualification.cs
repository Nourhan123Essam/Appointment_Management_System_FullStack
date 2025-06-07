using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Qualification : BaseEntity
    {
        public int Id { get; set; }
        public int YearEarned { get; set; } // Example: 2015

        public int DoctorId { get; set; } // Foreign Key to Doctor
        public Doctor Doctor { get; set; } = new(); // Navigation property
        public ICollection<QualificationTranslation> Translations { get; set; } = new List<QualificationTranslation>();

    }


}
