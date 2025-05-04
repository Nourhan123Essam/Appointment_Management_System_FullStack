namespace Appointment_System.Domain.Entities
{
    public class Specialization : BaseEntity
    {
        public int Id { get; set; } 
        public string Name { get; set; } = null!; // Name of the specialization (e.g., Cardiologist, Dentist)

        // Navigation property
        public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
    }

}
