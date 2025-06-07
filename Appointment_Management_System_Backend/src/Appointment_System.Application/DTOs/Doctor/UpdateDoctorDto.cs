namespace Appointment_System.Application.DTOs.Doctor
{
    public class UpdateDoctorDto
    {
        public int Id { get; set; }

        public List<int> SpecializationIds { get; set; } = new();

        public string Phone { get; set; }
        public string ImageUrl { get; set; }

        public decimal InitialFee { get; set; }
        public decimal FollowUpFee { get; set; }
        public int MaxFollowUps { get; set; }
        public int YearsOfExperience { get; set; }
    }

}
