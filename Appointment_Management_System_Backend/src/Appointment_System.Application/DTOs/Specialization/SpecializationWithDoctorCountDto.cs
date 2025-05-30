namespace Appointment_System.Application.DTOs.Specialization
{
    public class SpecializationWithDoctorCountDto
    {
        public int SpecializationId { get; set; }
        public string Name { get; set; } = null!;
        public int DoctorCount { get; set; }
    }

}
