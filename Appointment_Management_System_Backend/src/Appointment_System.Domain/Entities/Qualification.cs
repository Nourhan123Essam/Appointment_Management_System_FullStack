namespace Appointment_System.Domain.Entities
{
    public class Qualification : BaseEntity
    {
        public int Id { get; set; }
        public string QualificationName { get; set; } = null!; // Name of the qualification (e.g., MD, MSc, etc.)
        public string IssuingInstitution { get; set; } = null!; // (Optional) Example: Harvard Medical School
        public int YearEarned { get; set; } // Example: 2015

        public int DoctorId { get; set; } // Foreign Key to Doctor
        public Doctor Doctor { get; set; } // Navigation property
    }


}
